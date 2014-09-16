// Packing.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "math.h"
#include <iostream> 
#include <algorithm> 
#include <list>
#include <queue>
#include <memory>
#include "packing.h"



Packer::Packer(int width, int height)
{
	atlasWidth = width;
	atlasHeight = height;
	gap = 1;
	Rect rect(0, 0, atlasWidth, atlasHeight);
	freeList.push_back(rect);
	numAttempted = 0;
	abort = false;
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
	bool retval = RequestInternal(width, height, newRect, false);
	ClearBFSQueue();
	FreeExploreDataHeap();

	return retval;
}

bool Packer::RequestInternal(int width, int height, Rect& newRect, bool consolidatePass)
{
	list<Rect> newFreeRects;
	freeListLen = 0;
	numAttempted++;

	// Loop through free list looking for a single rectangle that fits
	// This whole case is unnecessary because the later Explore function will cover the same data set
	for (list<Rect>::iterator freeRect = freeList.begin(); freeRect != freeList.end(); freeRect++)
	{
		freeListLen++;
	}

	requestedWidth = width;
	requestedHeight = height;

	// Now try all merging all permutations of rectangles to find a fit
	// Start by exploring each rectangle individually
	for (list<Rect>::iterator i = freeList.begin(); i != freeList.end(); i++)
	{
		// Base exploration data
		Rect largestHoriz = *i;
		Rect largestVert = *i;
		list<Rect>* freeRectsBeingConsumedHoriz = new list<Rect>();
		list<Rect>* freeRectsBeingConsumedVert = new list<Rect>();
		freeRectsBeingConsumedHoriz->push_back(*i);
		freeRectsBeingConsumedVert->push_back(*i);
		ListOfRectListIterators* permutations = new ListOfRectListIterators();
		permutations->push_back(i);
		list<Rect>::iterator nextFromFreeList = i;
		nextFromFreeList++;

		ExploreData* data = AllocateExploreData();
		*data = ExploreData {
			permutations,
			1,
			largestHoriz,
			largestVert,
			freeRectsBeingConsumedHoriz,
			freeRectsBeingConsumedVert,
			nextFromFreeList
		};
		bfsQueue.push(data);
	}

	// Explore!
	while (bfsQueue.size() > 0 && !abort)
	{
		ExploreData* data = bfsQueue.front();
		bfsQueue.pop();

		bool retval = EvaluatePermuation(data, newRect);
		exploreDataHeap.push_back(data);

		if (retval)
		{
			allocatedList.push_back(newRect);
			ClearBFSQueue();

			if (!consolidatePass)
			{
				RepositionAllocated(newRect);
				//PackFreeSpaceDefunct();
			}
			return true;
		}
	}

	return false;
}

bool Packer::EvaluatePermuation(ExploreData* data, Rect& newRect)
{
	// Find the maximum size of contiguous rectangles favoring width
	// Also handles the base case of single rect/no combining
	if (ComputeHorizCaseDimensions(data->largestHoriz, *(*(data->permutation->rbegin())), data->freeRectsBeingConsumedHoriz) || data->length == 1)
	{
		if (EvaluateLargest(data->largestHoriz, data->freeRectsBeingConsumedHoriz, newRect))
		{
			return true;
		}
	}

	// Find the maximum size of contiguous rectangles favoring height
	if (ComputeVertCaseDimensions(data->largestVert, *(*(data->permutation->rbegin())), data->freeRectsBeingConsumedVert))
	{
		if (EvaluateLargest(data->largestVert, data->freeRectsBeingConsumedVert, newRect))
		{
			return true;
		}
	}

	// Didn't find any of this run length, so explore the next length of this permutation
	if (data->length < freeListLen)
	{
		for (ListOfRectListIterators::iterator i = data->permutation->begin(); i != data->permutation->end(); i++)
		{
			if (data->nextFromFreeList == freeList.end())  // Out of permutations to explore in this branch
				return false;

			// Copy iterator list.  Source of perf hit!!
			ListOfRectListIterators* nextPermutation = new ListOfRectListIterators();
			for (auto j = data->permutation->begin(); j != data->permutation->end(); j++)
			{
				nextPermutation->push_back(*j);
			}
			nextPermutation->push_back(data->nextFromFreeList);

			list<Rect>::iterator nextnextFromFreeList = data->nextFromFreeList;
			nextnextFromFreeList++;

			ExploreData* nextData = AllocateExploreData();
			*nextData = ExploreData {
				nextPermutation,
				data->length + 1,
				data->largestHoriz,
				data->largestVert,
				data->freeRectsBeingConsumedHoriz,
				data->freeRectsBeingConsumedVert,
				nextnextFromFreeList
			};

			bfsQueue.push(nextData);
		}
	}
	return false;
}

bool Packer::EvaluateLargest(Rect largestHoriz, list<Rect>* freeRectsBeingConsumed, Rect& newRect)
{
	// Try creating rect
	list<Rect> newFreeRects;
	if (TryCreateSubRect(largestHoriz, requestedWidth, requestedHeight, newRect))
	{
		// Clip this rect by the largest region rect
		Rect remainingLargest = largestHoriz;
		for (auto i = freeRectsBeingConsumed->begin(); i != freeRectsBeingConsumed->end(); i++)
		{
			// Clip all consumed rects by the newly allocated rect
			Clip(newRect, *i, newFreeRects);
		}
		ListMaintanceAfterCreate(largestHoriz, newFreeRects, freeRectsBeingConsumed);
		return true;
	}

	return false;
}


// Rect of form
//  X                             X
//  XYYY                       YYYX
//  X                             X
bool Packer::ComputeHorizCaseDimensions(Rect& largest, Rect rect, list<Rect>* freeRectsBeingConsumed)
{
	if (abs(largest.x2 - rect.x1) <= 1 || abs(largest.x1 - rect.x2) <= 1)
	{
		largest.x1 = min(largest.x1, rect.x1);
		largest.y1 = max(largest.y1, rect.y1);
		largest.x2 = max(largest.x2, rect.x2);
		largest.y2 = min(largest.y2, rect.y2);
		freeRectsBeingConsumed->push_back(rect);
		return true;
	}

	// Base length = 1 case
	if (largest.Equals(rect))
	{
		return true;
	}

	return false;
}

// Rect of form
//     X             YYY
//     X              X
//    YYY             X
bool Packer::ComputeVertCaseDimensions(Rect& largest, Rect rect, list<Rect>* freeRectsBeingConsumed)
{
	if (abs(largest.y2 - rect.y1) <= 1 || abs(largest.y1 - rect.y2) <= 1)
	{
		largest.x1 = max(largest.x1, rect.x1);
		largest.y1 = min(largest.y1, rect.y1);
		largest.x2 = min(largest.x2, rect.x2);
		largest.y2 = max(largest.y2, rect.y2);
		freeRectsBeingConsumed->push_back(rect);
		return true;
	}

	return false;
}

void Packer::EraseFromFreeList(Rect rect)
{
	for (auto i = freeList.begin(); i != freeList.end(); i++)
	{
		if (i->Equals(rect))
		{
			freeList.erase(i);
			return;
		}
	}
}

void Packer::ListMaintanceAfterCreate(Rect largest, list<Rect> newFreeRects, list<Rect>* freeRectsBeingConsumed)
{
	// Add new clipped free rects resulting from the chop
	for (auto newFreeRectIter = newFreeRects.begin(); newFreeRectIter != newFreeRects.end(); newFreeRectIter++)
		freeList.push_back(*newFreeRectIter);

	// Remove rectangles that were chopped
	for (auto i = freeRectsBeingConsumed->begin(); i != freeRectsBeingConsumed->end(); i++)
		EraseFromFreeList(*i);
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

bool compare_rect(const Rect& first, const Rect& second)
{
	return first.Size() > second.Size();
}

// Reposition all allocated rects sorted with the largest in the top most corner
void Packer::RepositionAllocated(Rect& newRect)
{
	list<Rect> sorted(allocatedList);
	sorted.sort(compare_rect);

	allocatedList.clear();
	freeList.clear();
	freeList.push_back(Rect(0, 0, atlasWidth, atlasHeight)); // -1 ?

	for (list<Rect>::iterator i = sorted.begin(); i != sorted.end(); i++)
	{
		Rect newNewRect;
		bool retval = RequestInternal(i->Width(), i->Height(), newNewRect, true);
		if (!retval)
			cout << "RepositionAllocated: ERROR rect that fit before does not fit now!";
		else
		{
			// Return the repositioned newly allocated rect
			if (i->Equals(newRect))
				newRect = newNewRect;
		}
	}
}

// This was an idea to consolidate free space
void Packer::PackFreeSpaceDefunct()
{
	// For all pairs of rects, find largest size
	for (list<Rect>::iterator i = freeList.begin(); i != freeList.end(); i++)
	{
		for (list<Rect>::iterator j = i; j != freeList.end(); j++)
		{
			// No sense combining with yourself
			if (i == j)
				continue;

			Rect largest, largestVert, largestHoriz = largestVert = *i;
			list<Rect>* freeRectsBeingConsumedHoriz = new list<Rect>();
			list<Rect>* freeRectsBeingConsumedVert = new list<Rect>();
			list<Rect>* freeRectsBeingConsumed = NULL;
			freeRectsBeingConsumedHoriz->push_back(*i);
			freeRectsBeingConsumedVert->push_back(*i);

			bool horizRect = ComputeHorizCaseDimensions(largestHoriz, *j, freeRectsBeingConsumedHoriz);
			if (horizRect)
			{
				if (largestHoriz.y1 >= largestHoriz.y2) // Move into Compute Dimensions?
					horizRect = false;
				if (largestHoriz.Size() <= i->Size() || largestHoriz.Size() <= j->Size())
					horizRect = false;
			}

			bool vertRect = ComputeVertCaseDimensions(largestVert, *j, freeRectsBeingConsumedVert);
			if (vertRect)
			{
				if (largestVert.x1 >= largestVert.x2) // Move into Compute Dimensions?
					vertRect = false;
				if (largestVert.Size() <= i->Size() || largestVert.Size() <= j->Size())
					vertRect = false;
			}

			if (horizRect && vertRect)
			{
				if (largestHoriz.Size() > largestVert.Size())
				{
					vertRect = false;
				}
				else
				{
					horizRect = false;
				}
			}

			if (horizRect)
			{
				largest = largestHoriz;
				freeRectsBeingConsumed = freeRectsBeingConsumedHoriz;
			}

			if (vertRect)
			{
				largest = largestVert;
				freeRectsBeingConsumed = freeRectsBeingConsumedVert;
			}

			if (horizRect || vertRect)
			{
				list<Rect> newFreeRects;
				Clip(largest, *i, newFreeRects);
				Clip(largest, *j, newFreeRects);
				newFreeRects.push_back(largest);
				ListMaintanceAfterCreate(largestHoriz, newFreeRects, freeRectsBeingConsumed);
			}
		}
	}
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

void Packer::ClearBFSQueue()
{
	// Clear queue 
	while (!bfsQueue.empty())
	{
		delete bfsQueue.front();
		bfsQueue.pop();
	}
}


void Packer::FreeExploreDataHeap()
{
	// Clear list 
	while (!exploreDataHeap.empty())
	{
		delete exploreDataHeap.front()->permutation;
		delete exploreDataHeap.front();
		exploreDataHeap.pop_front();
	}
}

Packer::ExploreData* Packer::AllocateExploreData()
{
	if (!exploreDataHeap.empty())
	{
		ExploreData* retval = exploreDataHeap.front();
		exploreDataHeap.pop_front();
		return retval;
	}
	return new ExploreData();
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

