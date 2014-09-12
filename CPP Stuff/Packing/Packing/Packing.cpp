// Packing.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "math.h"
#include <iostream> 
#include <algorithm> 
#include <list>
using namespace std;

struct Rect
{
	Rect()
	{
		x1 = y1 = x2 = y2 = 0;
	}

	Rect(int xa1, int ya1, int xa2, int ya2)
	{
		x1 = xa1; y1 = ya1; x2 = xa2; y2 = ya2;
	}

	bool Intersect(Rect rect2)
	{
		if (x1 >= rect2.x1 && x1 <= rect2.x2 &&
			y1 >= rect2.y1 && y1 <= rect2.y2)
			return true; // Rects top left point is contained with rect2
		if (x2 >= rect2.x1 && x2 <= rect2.x2 &&
			y2 >= rect2.y1 && y2 <= rect2.y2)
			return true; // Rects lower right point is contained in rect2

		if (rect2.x1 >= x1 && rect2.x1 <= x2 &&
			rect2.y1 >= y1 && rect2.y1 <= y2)
			return true; // rect 2's top left point contained in this rect

		if (rect2.x2 >= x1 && rect2.x2 <= x2 &&
			rect2.y2 >= y1 && rect2.y2 <= y2)
			return true; // rect 2's lower right point contained in this rect.  Unnecessary check?

		return false;
	}

	int x1, y1, x2, y2;
};


typedef list<list<Rect>::iterator> ListOfIterators;

class Packer
{
private:
	list<Rect> freeList;
	int freeListLen;
	int atlasWidth;
	int atlasHeight;

public:
	Packer(int width, int height)
	{
		atlasWidth = width;
		atlasHeight = height;
		Rect rect(0, 0, atlasWidth, atlasHeight);
		freeList.push_back(rect);
	}

	void OutputFree()
	{
		std::cout << "Free list\n";
		for (list<Rect>::iterator freeRect = freeList.begin(); freeRect != freeList.end(); freeRect++)
		{
			std::cout << "  rect(" << freeRect->x1 << ", " << freeRect->y1 << ", " << freeRect->x2 << ", " << freeRect->y2 << ")\n";
		}
	}

	bool Request(int width, int height, Rect& newRect)
	{
		list<Rect> newFreeRects;
		freeListLen = 0;

		// Loop through free list looking for a single rectangle that fits
		// This case is unnecessary because the later Explore function will cover the same data set
		for (list<Rect>::iterator freeRect = freeList.begin(); freeRect != freeList.end(); freeRect++)
		{
			if (TryCreateSubRect(*freeRect, width, height, newFreeRects))
			{
				newRect = Rect(freeRect->x1, freeRect->y1, freeRect->x1 + width, freeRect->y1 + height);
				for (auto newFreeRectIter = newFreeRects.begin(); newFreeRectIter != newFreeRects.end(); newFreeRectIter++)
					freeList.push_back(*newFreeRectIter);
				freeList.erase(freeRect);
				return true;
			}
			freeListLen++;
		}

		// Now try all merging all permutations of rectangles to find a fit
		// Start by exploring each rectangle individually
		for (list<Rect>::iterator i = freeList.begin(); i != freeList.end(); i++)
		{
			ListOfIterators iterators;
			iterators.push_back(i);
			list<Rect>::iterator nextFromFreeList = i;
			nextFromFreeList++;
			bool retval = Explore(iterators, width, height, 1, nextFromFreeList, newRect);
			if (retval)
				return true;
		}

		return false;
	}

private:

	bool EnsureAllIntersect(Rect& largest, ListOfIterators::iterator iterators, int length)
	{
		int listLen = 0;
		// Ensure all rectangles were adjacent
		for (list<Rect>::iterator rect = *iterators; rect != freeList.end() && listLen++ < length; rect++)
		{
			if (!largest.Intersect(*rect))
				return false;
		}

		return true;
	}

	bool Explore(ListOfIterators& permutation, int width, int height, int length, list<Rect>::iterator nextFromFreeList, Rect& newRect)
	{
		// For each entry in the list of iterators
		for (ListOfIterators::iterator i = permutation.begin(); i != permutation.end(); i++)
		{
			ListOfIterators rectsInPlay;
			Rect largest = *(*i); // First rect in the list
			if (ComputeHorizCaseDimensions(largest, i, rectsInPlay, length))
			{
				// Try creating rect
				list<Rect> newFreeRects;
				if (TryCreateSubRect(largest, width, height, newFreeRects))
				{
					ListMaintanceAfterCreate(largest, width, height, newRect, newFreeRects, rectsInPlay);
					return true;
				}
			}

			rectsInPlay.clear();
			largest = *(*i); // First rect in the list
			if (ComputeVertCaseDimensions(largest, i, rectsInPlay, length))
			{
				// Try creating rect
				list<Rect> newFreeRects;
				if (TryCreateSubRect(largest, width, height, newFreeRects))
				{
					ListMaintanceAfterCreate(largest, width, height, newRect, newFreeRects, rectsInPlay);
					return true;
				}
			}
		}

		// Didn't find any of this run length, so explore the next length of each permutation
		if (length < freeListLen)
		{
			for (ListOfIterators::iterator i = permutation.begin(); i != permutation.end(); i++)
			{
				if (nextFromFreeList == freeList.end())  // Out of permutations to explore in this branch
					return false;

				// Copy iterator list
				ListOfIterators nextIterators;
				for (ListOfIterators::iterator c = permutation.begin(); c != permutation.end(); c++)
				{
					nextIterators.push_back(*c);
				}

				nextIterators.push_back(nextFromFreeList);
				list<Rect>::iterator nextnextFromFreeList = nextFromFreeList;
				nextnextFromFreeList++;
				bool retval = Explore(nextIterators, width, height, length + 1, nextnextFromFreeList, newRect);
				if (retval)
					return true;
			}
		}
		return false;
	}

	// Rect of form
	//  X                             X
	//  XYYY                       YYYX
	//  X                             X
	bool ComputeHorizCaseDimensions(Rect& largest, ListOfIterators::iterator iterators, ListOfIterators& rectsInPlay, int length)
	{
		int listLen = 0;

		// For each rectangle in the iterator's list
		for (list<Rect>::iterator rect = *iterators; rect != freeList.end() && listLen++ < length; rect++)
		{
			largest.x1 = min(largest.x1, rect->x1);
			largest.y1 = max(largest.y1, rect->y1);
			largest.x2 = max(largest.x2, rect->x2);
			largest.y2 = min(largest.y2, rect->y2);
			rectsInPlay.push_back(rect);
		}

		if (!EnsureAllIntersect(largest, iterators, length))
			return false;

		return true;
	}

	// Rect of form
	//     X             YYY
	//     X              X
	//    YYY             X
	bool ComputeVertCaseDimensions(Rect& largest, ListOfIterators::iterator iterators, ListOfIterators& rectsInPlay, int length)
	{
		int listLen = 0;

		// For each rectangle in the iterator's list
		for (list<Rect>::iterator rect = *iterators; rect != freeList.end() && listLen++ < length; rect++)
		{
			largest.x1 = max(largest.x1, rect->x1);
			largest.y1 = min(largest.y1, rect->y1);
			largest.x2 = min(largest.x2, rect->x2);
			largest.y2 = max(largest.y2, rect->y2);
			rectsInPlay.push_back(rect);
		}

		if (!EnsureAllIntersect(largest, iterators, length))
			return false;

		return true;
	}

	void ListMaintanceAfterCreate(Rect largest, int width, int height, Rect& newRect, list<Rect> newFreeRects, ListOfIterators& rectsInPlay)
	{
		newRect = Rect(largest.x1, largest.y1, largest.x1 + width, largest.y1 + height);

		// Add 1 or 2 free rects resulting from the chop
		for (auto newFreeRectIter = newFreeRects.begin(); newFreeRectIter != newFreeRects.end(); newFreeRectIter++)
			freeList.push_back(*newFreeRectIter);

		// Remove rectangles that were chopped
		for (ListOfIterators::iterator i = rectsInPlay.begin(); i != rectsInPlay.end(); i++)
			freeList.erase(*i);
	}

	bool TryCreateSubRect(Rect rect, int width, int height, list<Rect>& newFreeRects)
	{
		if (rect.x2 - rect.x1 >= width && rect.y2 - rect.y1 >= height)
		{
			// Allocate a free rect below the newly created rect that extends to the right bound of the original rect
			if (rect.y1 + height < rect.y2)
			{
				newFreeRects.push_back(Rect(rect.x1, rect.y1 + height + 1, rect.x2, rect.y2));
			}

			// Allocate a free rect to the right of the newly created rect 
			if (rect.x1 + width < rect.y2)
			{
				newFreeRects.push_back(Rect(rect.x1 + width + 1, rect.y1, rect.x2, rect.y1 + height));
			}

			return true;
		}
		return false;
	}
};

void GetRect(int width, int height, Packer* packer, list<Rect>& allocated)
{
	Rect newRect;
	if (packer->Request(width, height, newRect))
	{
		std::cout << "Allocated rect(" << newRect.x1 << ", " << newRect.y1 << ", " << newRect.x2 << ", " << newRect.y2 << ")\n";
		allocated.push_back(newRect);
	}
	else
	{
		std::cout << "Failed to allocate " << width << " x " << height << " rect\n";
	}
}

int _tmain(int argc, _TCHAR* argv[])
{
	Packer* packer = new Packer(10, 10);
	list<Rect> allocated;

	GetRect(4, 2, packer, allocated);

	packer->OutputFree();

	GetRect(5, 8, packer, allocated);

	packer->OutputFree();

	char key;
	std::cin >> key;

	return 0;
}

