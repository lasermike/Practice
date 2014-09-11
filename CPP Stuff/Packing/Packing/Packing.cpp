// Packing.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "math.h"
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
		x1 = xa1;
		y1 = ya1;
		x2 = xa2;
		y2 = ya2;
	}

	int x1, y1, x2, y2;
};

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

class Packer
{
private:
	list<Rect> freeList;

public:
	Packer(int atlasWidth, int atlasHeight)
	{
		Rect rect(0, 0, atlasWidth, atlasHeight);
		freeList.push_back(rect);
	
	}

	bool Request(int width, int height, Rect& newRect)
	{
		list<Rect> newFreeRects;

		// Loop through free list looking for a single rectangle that fits
		for (list<Rect>::iterator freeRect = freeList.begin(); freeRect != freeList.end(); freeRect++)
		{
			if (CreateSubRect(*freeRect, width, height, newFreeRects))
			{
				newRect = Rect(freeRect->x1, freeRect->y1, freeRect->x1 + width, freeRect->y1 + height);
				for (auto newFreeRectIter = newFreeRects.begin(); newFreeRectIter != newFreeRects.end(); newFreeRectIter++)
					freeList.push_back(*newFreeRectIter);
				freeList.erase(freeRect);
				freeList.sort(compare_rects);
				return true;
			}
		}

		// Now try increasing sized runs of free rectangles looking for a space that fits
		// Increasing runs rather than all permuations work because we keep the free list sorted 
		for (list<Rect>::iterator start = freeList.begin(); start != freeList.end(); start++)
		{
			int left = start->x1;
			int top = start->y1;
			int right = start->x2;
			int bottom = start->y2;

			list<Rect>::iterator freeRect;

			// Find the largest contiguous horitizonal area
			for (freeRect = freeList.begin(); freeRect != start; freeRect++)
			{
				if (freeRect->x1 == right + 1)
				{
					right = freeRect->x2;
					top = max(top, freeRect->y1);
					bottom = min(bottom, freeRect->y2);
				}
			}

			// Try creating rect
			if (CreateSubRect(Rect(left, top, right, bottom), width, height, newFreeRects))
			{
				newRect = Rect(freeRect->x1, freeRect->y1, freeRect->x1 + width, freeRect->y1 + height);
				for (auto newFreeRectIter = newFreeRects.begin(); newFreeRectIter != newFreeRects.end(); newFreeRectIter++)
					freeList.push_back(*newFreeRectIter);
				freeList.erase(freeRect);
				freeList.sort(compare_rects);
				return true;
			}

			left = start->x1;
			top = start->y1;
			right = start->x2;
			bottom = start->y2;
			// Find the largest contiguous vertical area
			for (freeRect = freeList.begin(); freeRect != start; freeRect++)
			{
				if (freeRect->y1 == bottom + 1)
				{
					bottom = freeRect->y2;
					right = min(right, freeRect->x2);
					left = max(left, freeRect->x1);
				}
			}

			// Try creating rect
			if (CreateSubRect(Rect(left, top, right, bottom), width, height, newFreeRects))
			{
				newRect = Rect(freeRect->x1, freeRect->y1, freeRect->x1 + width, freeRect->y1 + height);
				for (auto newFreeRectIter = newFreeRects.begin(); newFreeRectIter != newFreeRects.end(); newFreeRectIter++)
					freeList.push_back(*newFreeRectIter);
				freeList.erase(freeRect);
				freeList.sort(compare_rects);
				return true;
			}

		}

		return false;
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

int _tmain(int argc, _TCHAR* argv[])
{
	Packer* packer = new Packer(100, 100);
	
	Rect newRect;
	if (packer->Request(20, 10, newRect))
	{
	}

	return 0;
}



