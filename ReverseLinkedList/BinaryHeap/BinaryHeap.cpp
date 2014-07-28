// BinaryHeap.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "queue"
#include "array"
#include "iostream"
using namespace std;

class Node
{
public:
	Node() : value(0), left(NULL), right(NULL)
	{

	}
	Node(int value) : Node()
	{
		this->value = value;
	}

	//private:
	int value;
	Node* left;
	Node* right;

};

template<typename T> class Heap
{
public:
	virtual T& Add(int value) = 0;
	virtual void Display() = 0;

protected:
	void padding(char ch, int n)
	{
		for (int i = 0; i < n; i++)
		{
			cout << ch;
		}
	}
};

class NodeHeap : public Heap<NodeHeap>
{
public:
	NodeHeap() { }

	NodeHeap& Add(int value)
	{
		Node* newNode = new Node(value);
		if (!head)
		{
			head = newNode;
			cout << "Add head\r\n";
		}
		else
		{
			Insert(head, newNode);
			cout << "Add node" << endl;
		}
		return *this;
	}

	void Display()
	{
		if (!head) return;

		DisplayNodeAndChildren(head);
		cout << endl;
	}

	void DisplayLevelOrder()
	{
		if (!head) return;

		queue<Node*> currentLevel;
		queue<Node*> nextLevel;
		
		currentLevel.push(head);
		while (!currentLevel.empty())
		{
			Node* node = currentLevel.front();
			currentLevel.pop();
			if (node)
			{
				cout << node->value << " ";
				nextLevel.push(node->left);
				nextLevel.push(node->right);
			}

			if (currentLevel.empty())
			{
				cout << endl;
				swap(nextLevel, currentLevel);
			}

		}
	}

	void Structure()
	{
		structure(head, 0);
	}

private:

	void structure(Node *root, int level)
	{
		if (root == NULL)
		{
			padding('\t', level);
			puts("~");
		}
		else
		{
			structure(root->right, level + 1);
			padding('\t', level);
			printf("%d\n", root->value);
			structure(root->left, level + 1);
		}
	}


	void DisplayNodeAndChildren(Node* current)
	{
		if (current)
		{
			cout << current->value << " ";
			DisplayNodeAndChildren(current->left);
			DisplayNodeAndChildren(current->right);
		}
	}

	void Insert(Node* root, Node* newNode)
	{
		queue<Node*> currentLevel, nextLevel;
		Node* parent = NULL;

		currentLevel.push(root);
		while (!currentLevel.empty())
		{
			Node* node = currentLevel.front();
			currentLevel.pop();
			if (!node->left)
			{
				node->left = newNode;
				parent = node;
				break;
			}
			if (!node->right)
			{
				node->right = newNode;
				parent = node;
  				break;
			}
			nextLevel.push(node->left);
			nextLevel.push(node->right);

			if (currentLevel.empty())
			{
				swap(nextLevel, currentLevel);
			}
		}

		//Structure();
		Sift(root, NULL, newNode);
	}

	bool Sift(Node* current, Node* parent, Node* newNode)
	{
		bool done = false;

		if (current == newNode)
		{
			if (current->value < parent->value)
			{
				swap(current->value, parent->value);
			}
			return true;
		}

		if (current->left)
		{
			done = Sift(current->left, current, newNode);
		}

		if (!done && current->right)
		{
			done = Sift(current->right, current, newNode);
		}

		if (done && current && parent)
		{
			if (current->value < parent->value)
			{
				swap(current->value, parent->value);
			}
		}
		return done;
	}

	Node* head = NULL;
};


class ArrayHeap : public Heap<ArrayHeap>
{
public:
	ArrayHeap() { }

	ArrayHeap& Add(int value)
	{
		data[++last] = value;
		Sift(last);
		return *this;
	}

	void Display()
	{
		for (int i = 0; i <= last; i++)
		{
			cout << data[i] << " ";
		}
		
		cout << endl;
	}

	void Structure()
	{
		structure(0, 0);
	}

private:
	void Sift(int index)
	{
		int parent = GetParent(index);
		if (parent >= 0)
		{
			if (data[index] < data[parent])
			{
				int tmp = data[index];
				data[index] = data[parent];
				data[parent] = tmp;
			}
			Sift(parent);
		}

	}

	void structure(int index, int level)
	{
		if (index > last)
		{
			padding('\t', level);
			puts("~");
		}
		else
		{
			structure(GetRightIndex(index), level + 1);
			padding('\t', level);
			printf("%d\n", data[index]);
			structure(GetLeftIndex(index), level + 1);
		}
	}


	int GetLeftIndex(int index)
	{
		return 2 * index + 1;
	}
	int GetRightIndex(int index)
	{
		return 2 * index + 2;
	}

	int GetParent(int index)
	{
		int foo = (index % 2 == 1) ? 1 : 2;

		return (index - foo) / 2;
	}

	array<int, 100> data;
	int last = -1;

};

int _tmain(int argc, _TCHAR* argv[])
{
	NodeHeap heap;

	heap.Add(1).Add(2).Add(3).Add(9).Add(7).Add(5).Add(4).Add(6).Add(0);
	
	heap.Add(8);
	heap.Display();
	cout << endl;

	heap.DisplayLevelOrder();
	heap.Structure();

	cout << "-----------------------\r\n";

	ArrayHeap arrayHeap;

	arrayHeap.Add(1).Add(2).Add(3).Add(9).Add(7).Add(5).Add(4).Add(6).Add(0);
	arrayHeap.Add(8);
	arrayHeap.Display();
	arrayHeap.Structure();

	return 0;
}

