// Nodes.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "iostream"
using namespace std;

class Node
{
public:
	Node() : value(0), next(NULL)
	{

	}
	Node(int value) : Node()
	{
		this->value = value;
	}

//private:
	int value;
	Node* next = NULL;

};

class LinkedList
{
public:
	LinkedList() : first(NULL) { };

	LinkedList& Add(int value)
	{
		Node* node = new Node(value);
		if (!first)
		{
			first = node;
		}
		else
		{
			FindLast()->next = node;
		}
		return *this;
	}

	Node* FindLast()
	{
		Node* node = first;
		while (node && node->next)
		{
			node = node->next;
		}
		return node;
	}

	void Display()
	{
		Node* node = first;

		if (node)
		{
			cout << node->value;
			node = node->next;
		}
		while (node)
		{
			cout << ", "  << node->value;
			node = node->next;
		}
		cout << "\r\n";
	}

	bool IsOrdered()
	{
		if (first && first->next)
		{
			Node* node = first;
			while (node->next)
			{
				if (node->value < node->next->value)
				{
					return false;
				}
				node = node->next;

			};
		}

		return true;
	}

	void Reverse()
	{
		if (first && first->next)
		{
			Node* node = first;
			Node* next = first->next;
			Node* prev = NULL;

			while (node)
			{
				next = node->next;
				node->next = prev;

				prev = node;
				node = next;
			}
			first = prev;
		}
	}
private:
	Node* first;
};

int _tmain(int argc, _TCHAR* argv[])
{
	LinkedList list;

	list.Add(1);
	list.Display();
	cout << "IsOrdered? " << (list.IsOrdered() ? "yes" : "no") << endl;

	list.Add(2).Add(3).Add(4).Add(2).Add(6);
	list.Display();
	cout << "IsOrdered? " << (list.IsOrdered() ? "yes" : "no") << "\r\n";

	list.Reverse();
	list.Display();

	return 0;
}

