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

	bool Request(int width, int height, Rect& newRect)
	{
		list<Rect> newFreeRects;
		freeListLen = 0;

		// Loop through free list looking for a single rectangle that fits
		// This case is unnecessary since the later explore function will cover the same data set
		for (list<Rect>::iterator freeRect = freeList.begin(); freeRect != freeList.end(); freeRect++)
		{
			if (CreateSubRect(*freeRect, width, height, newFreeRects))
			{
				newRect = Rect(freeRect->x1, freeRect->y1, freeRect->x1 + width, freeRect->y1 + height);
				for (auto newFreeRectIter = newFreeRects.begin(); newFreeRectIter != newFreeRects.end(); newFreeRectIter++)
					freeList.push_back(*newFreeRectIter);
				freeList.erase(freeRect);
				//freeList.sort(compare_rects);
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
			bool retval = Explore(iterators, width, height, 1, nextFromFreeList);
			if (retval)
				return true;
		}

		return false;
	}

	bool ComputeLeftCaseDimensions(Rect& largest, ListOfIterators::iterator iterators, ListOfIterators rectsInPlay, int length)
	{
		// For each rectangle in the iterator's list
		for (list<Rect>::iterator rect = *iterators; rect != freeList.end() && listLen++ < length; rect++)
		{
			largest.x1 = min(largest.x1, rect->x1);
			largest.y1 = max(largest.y1, rect->y1);
			largest.x2 = max(largest.x2, rect->x2);
			largest.y2 = min(largest.y2, rect->y2);
			rectsInPlay.push_back(rect);
		}

		// Non contigous rectangles will be inverted in either dimension
		if (largest.x1 >= largest.x2 || largest.y1 >= largest.y2)
			return false;
	}

	bool ComputeRightCaseDimensions(Rect& largest, ListOfIterators::iterator iterators, ListOfIterators rectsInPlay, int length)
	{
		return false;
	}

	bool ComputeTopCaseDimensions(Rect& largest, ListOfIterators::iterator iterators, ListOfIterators rectsInPlay, int length)
	{
		return false;
	}

	bool ComputeBottomCaseDimensions(Rect& largest, ListOfIterators::iterator iterators, ListOfIterators rectsInPlay, int length)
	{
		return false;
	}


	bool Explore(ListOfIterators iterators, int width, int height, int length, list<Rect>::iterator nextFromFreeList)
	{
		ListOfIterators rectsInPlay;

		// For each entry in the list of iterators
		for (ListOfIterators::iterator i = iterators.begin(); i != iterators.end(); i++)
		{
			Rect largest = *(*i); // First rect in the list
			ComputeLeftCaseDimensions(largest, i, rectsInPlay, length);

			// Try creating rect
			list<Rect> newFreeRects;
			if (CreateSubRect(largest, width, height, newFreeRects))
			{
				ListMaintanceAfterCreate(largest, width, height, newFreeRects, rectsInPlay);
				return true;
			}

			Rect largest = *(*i); // First rect in the list
			ComputeTopCaseDimensions(largest, i, rectsInPlay, length);

			// Try creating rect
			list<Rect> newFreeRects;
			if (CreateSubRect(largest, width, height, newFreeRects))
			{
				ListMaintanceAfterCreate(largest, width, height, newFreeRects, rectsInPlay);
				return true;
			}

			Rect largest = *(*i); // First rect in the list
			ComputeRightCaseDimensions(largest, i, rectsInPlay, length);

			// Try creating rect
			list<Rect> newFreeRects;
			if (CreateSubRect(largest, width, height, newFreeRects))
			{
				ListMaintanceAfterCreate(largest, width, height, newFreeRects, rectsInPlay);
				return true;
			}

			Rect largest = *(*i); // First rect in the list
			ComputeBottomCaseDimensions(largest, i, rectsInPlay, length);

			// Try creating rect
			list<Rect> newFreeRects;
			if (CreateSubRect(largest, width, height, newFreeRects))
			{
				ListMaintanceAfterCreate(largest, width, height, newFreeRects, rectsInPlay);
				return true;
			}

		}

		// Didn't find any of this run length, so explore the next length of each permutation
		if (length < freeListLen)
		{
			for (ListOfIterators::iterator i = iterators.begin(); i != iterators.end(); i++)
			{
				if (nextFromFreeList == freeList.end())  // Out of permutations to explore in this branch
					return false;

				// Copy iterator list
				ListOfIterators nextIterators;
				for (ListOfIterators::iterator c = iterators.begin(); c != iterators.end(); c++)
				{
					nextIterators.push_back(*c);
				}

				nextIterators.push_back(nextFromFreeList);
				list<Rect>::iterator nextnextFromFreeList = nextFromFreeList;
				nextnextFromFreeList++;
				Explore(nextIterators, width, height, length + 1, nextnextFromFreeList);
			}
		}
		return false;
	}

	void ListMaintanceAfterCreate(Rect largest, int width, int height, list<Rect> newFreeRects, ListOfIterators rectsInPlay)
	{
		Rect newRect = Rect(largest.x1, largest.y1, largest.x1 + width, largest.y1 + height);

		// Add 1 or 2 free rects resulting from the chop
		for (auto newFreeRectIter = newFreeRects.begin(); newFreeRectIter != newFreeRects.end(); newFreeRectIter++)
			freeList.push_back(*newFreeRectIter);

		// Remove rectangles that were chopped
		for (ListOfIterators::iterator i = rectsInPlay.begin(); i != rectsInPlay.end(); i++)
			freeList.erase(*i);
	}

private:

	bool CreateSubRect(Rect rect, int width, int height, list<Rect>& newFreeRects)
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
	GetRect(5, 10, packer, allocated);

	char key;
	std::cin >> key;

	return 0;
}



/*
bool compare_rects(const Rect& first, Rect& second)
{
	if (first.y1 == second.y1)
	{
		return first.x1 < first.y1;
	}
	else if (first.y1 < second.y1)
		return true;
	else return first.x1 < second.x1;
}
*/