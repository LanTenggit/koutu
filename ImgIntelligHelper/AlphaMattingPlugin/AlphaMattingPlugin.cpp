// AlphaMattingPlugin.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include "SharedMatting.h"
#include <time.h>

// 测试函数
void AlgorithmSample()
{
	char sInput[] = "E:/Vtotem/WPF/Demos/OpenCv/OpenCvCollections/MattingImgs/input1.jpg";
	char sTrimap[] = "E:/Vtotem/WPF/Demos/OpenCv/OpenCvCollections/MattingImgs/trimap1.jpg";
	char sOuput[] = "E:/Vtotem/WPF/Demos/OpenCv/OpenCvCollections/MattingImgs/AlphaMattingPlugin_Result1.jpg";
	clock_t start, finish;
	start = clock();

	SharedMatting sm;
	sm.loadImage(sInput);
	sm.loadTrimap(sTrimap);
	sm.solveAlpha();
	sm.save(sOuput);

	finish = clock();
	cout << "Total Interval Seconds";
	cout << double(finish - start) / (CLOCKS_PER_SEC * 2.5) << endl;
}

// 控制台应用函数入口，C#调用次进程的传参入口参数。
int main(int argc, char* args[])
{
	printf_s("输入的参数为：%s\n", args[1]);
	printf_s("输入的参数为：%s\n", args[2]);
	printf_s("输入的参数为：%s\n", args[3]);

	SharedMatting sm;
	sm.loadImage(args[1]);
	sm.loadTrimap(args[2]);
	sm.solveAlpha();
	sm.save(args[3]);
}
