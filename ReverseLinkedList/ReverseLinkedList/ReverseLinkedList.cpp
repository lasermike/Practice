// ReverseLinkedList.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "iostream"

struct Node
{
	int value;
	Node* next;
};

Node* _nodes = NULL;

void AddNode(int val)
{
	Node* newNode = new Node({ val, NULL });

	if (!_nodes)
	{
		_nodes = newNode;
		return;
	}
	else
	{
		Node* node = _nodes;
		Node* prev = NULL;

		while (node->next)
		{
			node = node->next;
		}
		node->next = newNode;
	}
}

void PrintList()
{
	Node* node = _nodes;
	while (node)
	{
		std::cout << node->value;
		std::cout << ", ";
		node = node->next;
	}
	std::cout << "\r\n";
}

void Reverse()
{
	Node* prev = NULL;
	Node* next = NULL;
	Node* node = _nodes;

	while(node)
	{
		next = node->next;
		node->next = prev;

		_nodes = node;

		prev = node;
		node = next;
	}


}

int _tmain(int argc, _TCHAR* argv[])
{
	int nodeValues[] = { 1, 2, 3, 4, 5 };

	std::cout << "Array\r\n";
	for (auto val : nodeValues)
	{
		std::cout << val;
		std::cout << ", ";
		AddNode(val);
	}
	std::cout << "\r\n";

	std::cout << "List\r\n";
	PrintList();

	Reverse();
	std::cout << "Reversed list\r\n";
	PrintList();

	Reverse();
	std::cout << "Reversed back list\r\n";
	PrintList();


	return 0;
}

