// Packing.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "math.h"
#include <iostream> 
#include <algorithm> 
#include <list>

#include "packing.h"



Packer::Packer(int width, int height)
{
	atlasWidth = width;
	atlasHeight = height;
	gap = 1;
	Rect rect(0, 0, atlasWidth, atlasHeight);
	freeList.push_back(rect);
	numAttempted = 0;
}

void Packer::OutputFree()
{
	cout << "Free list\n";
	for (list<Rect>::iterator freeRect = freeList.begin(); freeRect != freeList.end(); freeRect++)
	{
		cout << "  rect(" << freeRect->x1 << ", " << freeRect->y1 << ", " << freeRect->x2 << ", " << freeRect->y2 << ")\n";
	}
}

bool Packer::Request(int width, int height, Rect& newRect)
{
	list<Rect> newFreeRects;
	freeListLen = 0;
	numAttempted++;

	// Loop through free list looking for a single rectangle that fits
	// This whole case is unnecessary because the later Explore function will cover the same data set
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
			allocatedList.push_back(newRect);
			return true;
		}
		freeListLen++;
	}

	requestedWidth = width;
	requestedHeight = height;

	// Now try all merging all permutations of rectangles to find a fit
	// Start by exploring each rectangle individually
	for (list<Rect>::iterator i = freeList.begin(); i != freeList.end(); i++)
	{
		// Base recursion data
		Rect largestHoriz = *i;
		Rect largestVert = *i;
		ListOfIterators freeRectsBeingConsumedHoriz, freeRectsBeingConsumedVert;
		freeRectsBeingConsumedHoriz.push_back(i);
		freeRectsBeingConsumedVert.push_back(i);

		ListOfIterators iterators;
		iterators.push_back(i);
		list<Rect>::iterator nextFromFreeList = i;

		nextFromFreeList++;
		bool retval = Explore(iterators, 1, largestHoriz, largestVert, freeRectsBeingConsumedHoriz, freeRectsBeingConsumedVert, nextFromFreeList, newRect);
		if (retval)
		{
			allocatedList.push_back(newRect);
			return true;
		}
	}

	return false;
}

// Clips consumedRect by clipRect, adding intersection to newFreeRects
void Packer::Clip(Rect clipRect, Rect consumedRect, list<Rect>& newFreeRects)
{
	// Clip the top overlap off
	if (consumedRect.y1 < clipRect.y1)
	{
		Rect splitRect = consumedRect;
		splitRect.y2 = clipRect.y1 - gap;
		if (splitRect.Width() > 0)
			newFreeRects.push_back(splitRect);
	}

	// Clip bottom overlap
	if (consumedRect.y2 > clipRect.y2)
	{
		Rect splitRect = consumedRect;
		splitRect.y1 = clipRect.y2 + gap;
		if (splitRect.Width() > 0 && splitRect.Height() > 0)
			newFreeRects.push_back(splitRect);
	}

	// Clip left.  Assume top and bottom already cliped
	if (consumedRect.x1 < clipRect.x1)
	{
		Rect splitRect = consumedRect;
		splitRect.x2 = clipRect.x1 - gap;
		splitRect.y1 = max(clipRect.y1, splitRect.y1);
		splitRect.y2 = min(clipRect.y2, splitRect.y2);
		if (splitRect.Width() > 0 && splitRect.Height() > 0)
			newFreeRects.push_back(splitRect);
	}

	// Clip right.  Assume top and bottom already cliped
	if (consumedRect.x2 > clipRect.x2)
	{
		Rect splitRect = consumedRect;
		splitRect.x1 = clipRect.x2 + gap;
		splitRect.y1 = max(clipRect.y1, splitRect.y1);
		splitRect.y2 = min(clipRect.y2, splitRect.y2);
		if (splitRect.Width() > 0 && splitRect.Height() > 0)
			newFreeRects.push_back(splitRect);
	}
}

bool Packer::Explore(ListOfIterators& permutation, int length, Rect largestHoriz, Rect largestVert, ListOfIterators& freeRectsBeingConsumedHoriz, ListOfIterators& freeRectsBeingConsumedVert, list<Rect>::iterator nextFromFreeList, Rect& newRect)
{
	// Find the maximum size of contiguous rectangles favoring width
	if (ComputeHorizCaseDimensions(largestHoriz, *(permutation.rbegin()), freeRectsBeingConsumedHoriz, length))
	{
		// Try creating rect
		list<Rect> newFreeRects;
		if (TryCreateSubRect(largestHoriz, requestedWidth, requestedHeight, newRect))
		{
			// Clip this rect by the largest region rect
			Rect remainingLargest = largestHoriz;
			for (ListOfIterators::iterator i = freeRectsBeingConsumedHoriz.begin(); i != freeRectsBeingConsumedHoriz.end(); i++)
			{
				// Clip all consumed rects by the newly allocated rect
				Clip(newRect, *(*i), newFreeRects);
			}
			ListMaintanceAfterCreate(largestHoriz, requestedWidth, requestedHeight, newFreeRects, freeRectsBeingConsumedHoriz);
			return true;
		}
	}

	// Find the maximum size of contiguous rectangles favoring height
	if (ComputeVertCaseDimensions(largestVert, *(permutation.rbegin()), freeRectsBeingConsumedVert, length))
	{
		// Try creating rect
		list<Rect> newFreeRects;
		if (TryCreateSubRect(largestVert, requestedWidth, requestedHeight, newRect))
		{
			for (ListOfIterators::iterator i = freeRectsBeingConsumedVert.begin(); i != freeRectsBeingConsumedVert.end(); i++)
			{
				// Clip all consumed rects by the newly allocated rect
				Clip(newRect, *(*i), newFreeRects);
			}
			ListMaintanceAfterCreate(largestVert, requestedWidth, requestedHeight, newFreeRects, freeRectsBeingConsumedVert);
			return true;
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
			ListOfIterators nextIterators(permutation);

			nextIterators.push_back(nextFromFreeList);
			list<Rect>::iterator nextnextFromFreeList = nextFromFreeList;
			nextnextFromFreeList++;
			bool retval = Explore(nextIterators, length + 1, largestHoriz, largestVert, freeRectsBeingConsumedHoriz, freeRectsBeingConsumedVert, nextnextFromFreeList, newRect);
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
bool Packer::ComputeHorizCaseDimensions(Rect& largest, list<Rect>::iterator rect, ListOfIterators& freeRectsBeingConsumed, int length)
{
	if (abs(largest.x2 - rect->x1) <= 1 || abs(largest.x1 - rect->x2) <= 1)
	{
		largest.x1 = min(largest.x1, rect->x1);
		largest.y1 = max(largest.y1, rect->y1);
		largest.x2 = max(largest.x2, rect->x2);
		largest.y2 = min(largest.y2, rect->y2);
		freeRectsBeingConsumed.push_back(rect);
		return true;
	}

	return false;
}

// Rect of form
//     X             YYY
//     X              X
//    YYY             X
bool Packer::ComputeVertCaseDimensions(Rect& largest, list<Rect>::iterator rect, ListOfIterators& freeRectsBeingConsumed, int length)
{
	if (abs(largest.y2 - rect->y1) <= 1 || abs(largest.y1 - rect->y2) <= 1)
	{
		largest.x1 = max(largest.x1, rect->x1);
		largest.y1 = min(largest.y1, rect->y1);
		largest.x2 = min(largest.x2, rect->x2);
		largest.y2 = max(largest.y2, rect->y2);
		freeRectsBeingConsumed.push_back(rect);
		return true;
	}

	return false;
}

void Packer::ListMaintanceAfterCreate(Rect largest, int width, int height, list<Rect> newFreeRects, ListOfIterators& freeRectsBeingConsumed)
{
	// Add new clipped free rects resulting from the chop
	for (auto newFreeRectIter = newFreeRects.begin(); newFreeRectIter != newFreeRects.end(); newFreeRectIter++)
		freeList.push_back(*newFreeRectIter);

	// Remove rectangles that were chopped
	for (ListOfIterators::iterator i = freeRectsBeingConsumed.begin(); i != freeRectsBeingConsumed.end(); i++)
		freeList.erase(*i);
}

bool Packer::TryCreateSubRect(Rect largest, int width, int height, Rect& newRect)
{
	if (largest.x2 - largest.x1 >= width && largest.y2 - largest.y1 >= height)
	{
		newRect = Rect(largest.x1, largest.y1, largest.x1 + width, largest.y1 + height);
		return true;
	}
	return false;
}



bool PackerTest::GetRect(int width, int height, Packer* packer, Rect& newRect)
{
	bool retval = false;
	cout << "Requesting (" << width << " x " << height << ") rect\n";
	if (packer->Request(width, height, newRect))
	{
		std::cout << "Allocated rect(" << newRect.x1 << ", " << newRect.y1 << ", " << newRect.x2 << ", " << newRect.y2 << ")\n";
		retval = true;
	}
	else
	{
		std::cout << "Failed to allocate " << width << " x " << height << " rect\n";
	}
	packer->OutputFree();
	return retval;
}

// Test if given rectangle exists in free list
bool Packer::CheckFree(Rect rect)
{
	for (auto i = freeList.begin(); i != freeList.end(); i++)
	{
		if (rect.Equals(*i))
			return true;
	}
	cout << "CheckFree: Error rect ";
	rect.Print();
	cout << " not found\n";

	return false;
}


void PackerTest::Case1()
{
	Packer* packer = new Packer(10, 10);

	Rect newRect;
	PackerTest::GetRect(4, 2, packer, newRect);
	packer->OutputFree();
	packer->CheckFree(Rect(0, 3, 10, 10));
	packer->CheckFree(Rect(5, 0, 10, 2));

	PackerTest::GetRect(5, 8, packer, newRect);
	packer->OutputFree();
	packer->CheckFree(Rect(0, 3, 4, 8));
	packer->CheckFree(Rect(0, 9, 10, 10));
	free(packer);
}

void PackerTest::Case2()
{
	Packer* packer = new Packer(10, 10);

	Rect newRect;
	GetRect(2, 9, packer, newRect);
	packer->OutputFree();
	//packer->CheckFree(Rect(9, 0, 10, 2));
	//packer->CheckFree(Rect(3, 0, 10, 10));

	free(packer);
}

int _tmain(int argc, _TCHAR* argv[])
{
	PackerTest::Case1();
	PackerTest::Case2();

	char key;
	std::cin >> key;

	return 0;
}

