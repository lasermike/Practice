// FindFirstLoop.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <list>
#include <vector>
#include <stack>
#include <unordered_set>

using namespace std;

struct Node
{
public:
	Node() : value(0), children()
	{

	}
	Node(int value) : Node()
	{
		this->value = value;
	}

	Node* AddChild(int value)
	{
		Node* node = new Node(value);
		children.push_back(node);
		return node;
	}

	vector<Node*> children;
	int value;
};
 
/* -=-=-=-=-=- */

class Graph
{
private:
	
	bool _DetectLoop(Node* node, unordered_set<Node*>& visited, stack<Node*>& history)
	{
		if (!visited.insert(node).second)
		{
			// Loop found
			while (!history.empty())
			{
				cout << history.top()->value << endl;
				if (history.top()->value == node->value)
					break;
				history.pop();
			}
			return true;
		}
		history.push(node);
		for (unsigned int i = 0; i < node->children.size(); i++)
		{
			if (_DetectLoop((Node*)node->children[i], visited, history))
			{
				return true;
			}
		}
		return false;
	}

public:
	Graph() {}

	bool DetectLoop(Node* node)
	{
		unordered_set<Node*> stackvisited;
		stack<Node*> stackhistory;

		return _DetectLoop(node, stackvisited, stackhistory);
	}
};

int _tmain(int argc, _TCHAR* argv[])
{
	Graph graph;

	Node root(1);
	Node* n2 = root.AddChild(2);
	Node* n3 = root.AddChild(3);

	Node* n4 = n2->AddChild(4);
	n2->AddChild(9);

	n3->AddChild(6);
	n3->AddChild(5);

	Node* n7 = n4->AddChild(7);
	n4->AddChild(8);
	n7->children.push_back(n2);

	cout << (graph.DetectLoop(&root) ? "Detected loop\r\n" : "No loop\r\n");

	return 0;
}

