
#include <list>
#include <queue>
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

	const int Size()
	{
		return (x2 - x1) * (y2 - y1);
	}

};

typedef list<list<Rect>::iterator> ListOfRectListIterators;

class Packer
{
public:
	Packer(int width, int height);

	bool Request(int width, int height, Rect& newRect);

	const int GetWidth() const { return atlasWidth; }
	const int GetHeight() const { return atlasHeight; }

	// Debugging stats
	void OutputFree();
	bool CheckFree(Rect rect);
	const list<Rect>& GetFreeList() { return freeList; }
	const list<Rect>& GetAllocatedList() { return allocatedList; }
	int GetNumAttempted() { return numAttempted; }

private:

	struct ExploreData
	{
		ListOfRectListIterators* permutation;
		int length;
		Rect largestHoriz;
		Rect largestVert;
		ListOfRectListIterators freeRectsBeingConsumedHoriz;
		ListOfRectListIterators freeRectsBeingConsumedVert;
		list<Rect>::iterator nextFromFreeList;
	};

	list<Rect> freeList;
	list<Rect> allocatedList;
	int freeListLen;
	int atlasWidth;
	int atlasHeight;
	int gap;
	int numAttempted;

	// Some helpful temporary state used during Explore
	int requestedWidth;
	int requestedHeight;
	queue<ExploreData*> bfsQueue;

	bool RequestInternal(int width, int height, Rect& newRect);
	bool Explore(ExploreData* data, Rect& newRect);
	void PackFreeSpace();
	bool EvaluateLargest(Rect largestHoriz, ListOfRectListIterators& freeRectsBeingConsumed, Rect& newRect);
	bool ComputeHorizCaseDimensions(Rect& largest, list<Rect>::iterator rect, ListOfRectListIterators& freeRectsBeingConsumed);
	bool ComputeVertCaseDimensions(Rect& largest, list<Rect>::iterator rect, ListOfRectListIterators& freeRectsBeingConsumed);
	bool TryCreateSubRect(Rect largest, int width, int height, Rect& newRect);
	void Clip(Rect clipRect, Rect consumedRect, list<Rect>& newFreeRects);
	void ListMaintanceAfterCreate(Rect largest, list<Rect> newFreeRects, ListOfRectListIterators& freeRectsBeingConsumed);
};



class PackerTest
{
public:
	static void Case1();
	static void Case2();
	static bool GetRect(int width, int height, Packer* packer, Rect& newRect);
};
