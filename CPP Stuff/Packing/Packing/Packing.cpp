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

	bool Equals(Rect rect)
	{
		return rect.x1 == x1 && rect.x2 == x2 && rect.y1 == y1 && rect.y2 == y2;
	}

	void Print()
	{
		cout << "(" << x1 << ", " << y1 << ", " << x2 << ", " << y2 << ")";
	}
	int Width()
	{
		return x2 - x1;
	}

	int Height()
	{
		return y2 - y1;
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
		cout << "Free list\n";
		for (list<Rect>::iterator freeRect = freeList.begin(); freeRect != freeList.end(); freeRect++)
		{
			cout << "  rect(" << freeRect->x1 << ", " << freeRect->y1 << ", " << freeRect->x2 << ", " << freeRect->y2 << ")\n";
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
			if (TryCreateSubRect(*freeRect, width, height, newRect))
			{
				// Clip all consumed rects by the newly allocated rect
				Clip(newRect, *freeRect, newFreeRects);

				//newRect = Rect(freeRect->x1, freeRect->y1, freeRect->x1 + width, freeRect->y1 + height);
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

	void CheckFree(Rect rect)
	{
		for (auto i = freeList.begin(); i != freeList.end(); i++)
		{
			if (rect.Equals(*i))
				return;
		}
		cout << "CheckFree: Error rect ";
		rect.Print();
		cout << " not found\n";
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

	// Clips consumedRect by clipRect, adding intersection to newFreeRects
	void Clip(Rect clipRect, Rect consumedRect, list<Rect>& newFreeRects)
	{
		Rect rect = consumedRect;

		// Case where this rect's height extends above new rect region
		//  Input X       Output     F
		//        X                  F
		//        XYY				 NNN
		//        X                  R
		if (rect.y1 < clipRect.y1)
		{
			//INCOMPLETE!!
			Rect newFree = Rect(rect.x1, rect.y1, rect.x2, clipRect.y1 - 1);
			if (newFree.Width() > 0 && newFree.Height() > 0)
				newFreeRects.push_back(newFree);
			rect = Rect(rect.x1, clipRect.y2 + 1, rect.x2, rect.y2); // Remainder
		}

		if (rect.y2 > clipRect.y2)
		{
			// Case where this rect's height extends below and left of clip region
			//  Input  X       Output     C
			//         X                  C
			//        YYY                1C2
			if (rect.x1 < clipRect.x1)
			{
				Rect newFree = Rect(rect.x1, rect.y1, clipRect.x1 - 1, rect.y2);
				if (newFree.Width() > 0 && newFree.Height() > 0)
				{
					newFreeRects.push_back(newFree);
					rect = Rect(clipRect.x1, clipRect.y2 + 1, rect.x2, rect.y2);  // Remainder
					if (rect.Width() > 0 && rect.Height() > 0)
						newFreeRects.push_back(rect);
				}
			}
			else
			{
				// Case where this rect's height extends below and right of clip region
				//  Input         Output      
				//          Y                 C
				//          YXX               CX
				//           XX               CX
				Rect newFree = Rect(rect.x1, clipRect.y2 + 1, rect.x2, rect.y2);
				if (newFree.Width() > 0 && newFree.Height() > 0)
					newFreeRects.push_back(newFree);
				rect = Rect(clipRect.x2 + 1, rect.y1, rect.x2, newFree.y1 - 1);  // Remainder
				if (rect.Width() > 0 && rect.Height() > 0)
					newFreeRects.push_back(rect);
			}
			return;
		}

		if (rect.x1 < clipRect.x1)
		{
			// Case where this rect's width extends lower left of clip region
			//  Input          Largest
			//          Y                  L
			//          Y                  L
			//        XXXX               XXLX
			if (rect.y1 < clipRect.y1)
			{
				Rect newFree = Rect(rect.x1, rect.y1, clipRect.x1 - 1, rect.y2);
				if (newFree.Width() > 0 && newFree.Height() > 0)
					newFreeRects.push_back(newFree);
				rect = Rect(clipRect.x1 + 1, rect.y1, rect.x2, rect.y2);  // Remainder
				if (rect.Width() > 0 && rect.Height() > 0)
					newFreeRects.push_back(rect);
			}
			else
			{
				// Case where this rect's width extends upper left of clip region
				//  Input   
				//        XXXX      Largest  XXLX
				//          Y                  L
				//          Y                  L
				Rect newFree = Rect(rect.x1, rect.y1, clipRect.x1 - 1, rect.y2);
				if (newFree.Width() > 0 && newFree.Height() > 0)
					newFreeRects.push_back(newFree);
				rect = Rect(clipRect.x1 + 1, rect.y1, rect.x2, rect.y2);  // Remainder
				if (rect.Width() > 0 && rect.Height() > 0)
					newFreeRects.push_back(rect);

			}
			return;
		}
		// Case where this rect's height extends right of largest region
		//  Input Y       Largest   L
		//        YXX               LXX
		//        Y                 L
		if (rect.x2 > clipRect.x2)
		{
			//INCOMPLETE!!
			Rect newFree = Rect(clipRect.x2 + 1, rect.y1, rect.x2, rect.y2);
			if (newFree.Width() > 0 && newFree.Height() > 0)
			{
				newFreeRects.push_back(newFree);
				//rect = Rect()
				if (rect.Width() > 0 && rect.Height() > 0)
					newFreeRects.push_back(rect);
			}
		}
	}

	bool Explore(ListOfIterators& permutation, int width, int height, int length, list<Rect>::iterator nextFromFreeList, Rect& newRect)
	{
		// For each entry in the list of iterators
		for (ListOfIterators::iterator i = permutation.begin(); i != permutation.end(); i++)
		{
			ListOfIterators freeRectsBeingConsumed;
			Rect largest = *(*i); // First rect in the list
			if (ComputeHorizCaseDimensions(largest, i, freeRectsBeingConsumed, length))
			{
				// Try creating rect
				list<Rect> newFreeRects;
				if (TryCreateSubRect(largest, width, height, newRect))
				{
					// Clip this rect by the largest region rect
					Rect remainingLargest = largest;
					for (ListOfIterators::iterator i = freeRectsBeingConsumed.begin(); i != freeRectsBeingConsumed.end(); i++)
					{
						// Clip all consumed rects by the newly allocated rect
						Clip(newRect, *(*i), newFreeRects);
					}
					ListMaintanceAfterCreate(largest, width, height, newFreeRects, freeRectsBeingConsumed);
					return true;
				}
			}

			freeRectsBeingConsumed.clear();
			largest = *(*i); // First rect in the list
			if (ComputeVertCaseDimensions(largest, i, freeRectsBeingConsumed, length))
			{
				// Try creating rect
				list<Rect> newFreeRects;
				if (TryCreateSubRect(largest, width, height, newRect))
				{
					for (ListOfIterators::iterator i = freeRectsBeingConsumed.begin(); i != freeRectsBeingConsumed.end(); i++)
					{
						// Clip all consumed rects by the newly allocated rect
						Clip(newRect, *(*i), newFreeRects);
					}
					ListMaintanceAfterCreate(largest, width, height, newFreeRects, freeRectsBeingConsumed);
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
	bool ComputeHorizCaseDimensions(Rect& largest, ListOfIterators::iterator iterators, ListOfIterators& freeRectsBeingConsumed, int length)
	{
		int listLen = 0;

		// For each rectangle in the iterator's list
		for (list<Rect>::iterator rect = *iterators; rect != freeList.end() && listLen++ < length; rect++)
		{
			largest.x1 = min(largest.x1, rect->x1);
			largest.y1 = max(largest.y1, rect->y1);
			largest.x2 = max(largest.x2, rect->x2);
			largest.y2 = min(largest.y2, rect->y2);
			freeRectsBeingConsumed.push_back(rect);
		}

		// Note to double check this is actually sufficient to determine if all rectangles were adjacent
		if (!EnsureAllIntersect(largest, iterators, length))
			return false;

		return true;
	}

	// Rect of form
	//     X             YYY
	//     X              X
	//    YYY             X
	bool ComputeVertCaseDimensions(Rect& largest, ListOfIterators::iterator iterators, ListOfIterators& freeRectsBeingConsumed, int length)
	{
		int listLen = 0;

		// For each rectangle in the iterator's list
		for (list<Rect>::iterator rect = *iterators; rect != freeList.end() && listLen++ < length; rect++)
		{
			largest.x1 = max(largest.x1, rect->x1);
			largest.y1 = min(largest.y1, rect->y1);
			largest.x2 = min(largest.x2, rect->x2);
			largest.y2 = max(largest.y2, rect->y2);
			freeRectsBeingConsumed.push_back(rect);
		}

		if (!EnsureAllIntersect(largest, iterators, length))
			return false;

		return true;
	}

	void ListMaintanceAfterCreate(Rect largest, int width, int height, list<Rect> newFreeRects, ListOfIterators& freeRectsBeingConsumed)
	{
		// Add new clipped free rects resulting from the chop
		for (auto newFreeRectIter = newFreeRects.begin(); newFreeRectIter != newFreeRects.end(); newFreeRectIter++)
			freeList.push_back(*newFreeRectIter);

		// Remove rectangles that were chopped
		for (ListOfIterators::iterator i = freeRectsBeingConsumed.begin(); i != freeRectsBeingConsumed.end(); i++)
			freeList.erase(*i);
	}

	bool TryCreateSubRect(Rect largest, int width, int height, Rect& newRect)
	{
		if (largest.x2 - largest.x1 >= width && largest.y2 - largest.y1 >= height)
		{
			newRect = Rect(largest.x1, largest.y1, largest.x1 + width, largest.y1 + height);
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

void Case1()
{
	Packer* packer = new Packer(10, 10);
	list<Rect> allocated;

	GetRect(4, 2, packer, allocated);
	packer->OutputFree();
	packer->CheckFree(Rect(0, 3, 10, 10));
	packer->CheckFree(Rect(5, 0, 10, 2));

	GetRect(5, 8, packer, allocated);
	packer->OutputFree();
	packer->CheckFree(Rect(0, 3, 4, 10));
	packer->CheckFree(Rect(5, 9, 10, 10));
	free(packer);
}

void Case2()
{
	Packer* packer = new Packer(10, 10);
	list<Rect> allocated;

	GetRect(2, 9, packer, allocated);
	packer->OutputFree();
	//packer->CheckFree(Rect(9, 0, 10, 2));
	//packer->CheckFree(Rect(3, 0, 10, 10));

	free(packer);
}

int _tmain(int argc, _TCHAR* argv[])
{
	Case1();
	Case2();

	char key;
	std::cin >> key;

	return 0;
}

