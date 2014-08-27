// TinyHeap.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <tuple>
#include <list>
#include <functional>
#include <array>

typedef std::tuple<int, int> Range;


inline int GetStart(Range& block)
{
	return std::get<0>(block);
}

inline int GetEnd(Range& block)
{
	return std::get<1>(block);
}

bool rangeLessThan(Range& r1, Range& r2)
{
	return GetStart(r1) < GetStart(r2);
}

class TinyHeap
{

private:
	char* data = NULL;
	int dataLen = 0;
	std::list<Range> allocatedBlocks;

	TinyHeap() { }
	TinyHeap(char* memory, int memorySize) : data(memory), dataLen(memorySize) 	{	}

	char* AddAllocatedBlock(int start, int end)
	{
		allocatedBlocks.push_back(Range(start, end));
		allocatedBlocks.sort(rangeLessThan);
		return data + start;
	}

	char* AllocBlock(int bytesNeeded)
	{
		std::list<Range>::iterator i;
		int start = 0, end = bytesNeeded - 1;

		for (i = allocatedBlocks.begin(); i != allocatedBlocks.end() && end < dataLen; i++)
		{
			if (start < GetStart(*i) && end < GetStart(*i))
			{
				break;
			}

			start = GetEnd(*i);
			end = start + (bytesNeeded - 1);
		}

		if (end < dataLen)
		{
			return AddAllocatedBlock(start, end);
		}

		return NULL;
	}

public:

	char* TinyAlloc(int bytesNeeded)
	{
		return AllocBlock(bytesNeeded);
	}

	void TinyFree(char* location)
	{
		for (auto block : allocatedBlocks)
		{
			if (GetStart(block) == location - data)
			{
				allocatedBlocks.remove(block);
				allocatedBlocks.sort(rangeLessThan);
				return;
			}
		}

		std::cout << "ERROR freeing block" << std::endl;

		return;
	}

	static TinyHeap* AllocTinyHeap(char* memory, int memorySize)
	{
		return new TinyHeap(memory, memorySize);
	}
	
	static void DeleteTinyHeap(TinyHeap* heap)
	{
		// ???
		if (heap->data != NULL)
		{
		}
	}



};

char* TestCreateBlock(TinyHeap* heap, int size)
{
	char* newBlock = heap->TinyAlloc(size);

	if (newBlock)
	{
		std::cout << "Allocated " << size << " at " << ((int)newBlock) << std::endl;
	}
	else
	{
		std::cout << "Could not allocate " << (int) newBlock << std::endl;
	}

	return newBlock;
}

int _tmain(int argc, _TCHAR* argv[])
{
	char data[100];
	TinyHeap* heap = TinyHeap::AllocTinyHeap(data, 100);

	std::array<Range, 10> blocks;

	for (int i = 0; i < 10; i++)
	{
		char* newBlock = TestCreateBlock(heap, 10);
		if (newBlock)
		{
			blocks[i] = Range((int)newBlock, (int)(newBlock + 10));
		}

		
	}

	heap->TinyFree((char*)GetStart(blocks[2]));
	heap->TinyFree((char*)GetStart(blocks[5]));

	TestCreateBlock(heap, 8);
	TestCreateBlock(heap, 8);
	TestCreateBlock(heap, 8);
	TestCreateBlock(heap, 8);

	return 0;
}

