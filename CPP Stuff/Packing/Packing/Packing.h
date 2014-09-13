
#include <list>
#include <iostream>
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
		std::cout << "(" << x1 << ", " << y1 << ", " << x2 << ", " << y2 << ")";
	}
	const int Width() const
	{
		return x2 - x1;
	}

	const int Height() const
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
	list<Rect> allocatedList;
	int freeListLen;
	int atlasWidth;
	int atlasHeight;
	int gap;

public:
	Packer(int width, int height);
	bool Request(int width, int height, Rect& newRect);

	void OutputFree();
	void CheckFree(Rect rect);
	const int GetWidth() const { return atlasWidth; }
	const int GetHeight() const { return atlasHeight; }
	const list<Rect>& GetFreeList() { return freeList; }
	const list<Rect>& GetAllocatedList() { return allocatedList; }

private:

	bool EnsureAllIntersect(Rect& largest, ListOfIterators::iterator iterators, int length);
	void Clip(Rect clipRect, Rect consumedRect, list<Rect>& newFreeRects);
	bool Explore(ListOfIterators& permutation, int width, int height, int length, list<Rect>::iterator nextFromFreeList, Rect& newRect);
	bool ComputeHorizCaseDimensions(Rect& largest, ListOfIterators::iterator iterators, ListOfIterators& freeRectsBeingConsumed, int length);
	bool ComputeVertCaseDimensions(Rect& largest, ListOfIterators::iterator iterators, ListOfIterators& freeRectsBeingConsumed, int length);
	void ListMaintanceAfterCreate(Rect largest, int width, int height, list<Rect> newFreeRects, ListOfIterators& freeRectsBeingConsumed);
	bool TryCreateSubRect(Rect largest, int width, int height, Rect& newRect);

};



class PackerTest
{
public:
	static void Case1();
	static void Case2();
	static bool GetRect(int width, int height, Packer* packer, Rect& newRect);
};
