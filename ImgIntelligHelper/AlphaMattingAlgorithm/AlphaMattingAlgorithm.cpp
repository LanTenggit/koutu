// AlphaMattingAlgorithm.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include "SharedMatting.h"
#include <time.h>

void Sample1()
{
	// Todo: 你需要根据你的路径修改下此路劲
	char sInput[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/input1.jpg";
	char sTrimap[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/trimap1.jpg";
	char sOuput[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/AlphaMattingAlgorithm2.jpg";
	clock_t start, finish;
	start = clock();

	SharedMatting sm;
	sm.loadImage(sInput);
	sm.loadTrimap(sTrimap);
	sm.solveAlpha();
	sm.save(sOuput);

	finish = clock();
	cout << "Total Interval Seconds ( input1.jpg ) : ";
	cout << double(finish - start) / (CLOCKS_PER_SEC * 2.5) << endl;
}

void Sample2()
{
	// Todo: 你需要根据你的路径修改下此路劲
	char sInput[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/input2.jpg";
	char sTrimap[]= "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/trimap2.jpg";
	char sOuput[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/AlphaMattingAlgorithm2.jpg";
	clock_t start, finish;
	start = clock();

	SharedMatting sm;
	sm.loadImage(sInput);
	sm.loadTrimap(sTrimap);
	sm.solveAlpha();
	sm.save(sOuput);

	finish = clock();
	cout << "Total Interval Seconds ( input2.jpg ) : ";
	cout << double(finish - start) / (CLOCKS_PER_SEC * 2.5) << endl;
}

void Sample3()
{
	// Todo: 你需要根据你的路径修改下此路劲
	char sInput[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/input3.jpg";
	char sTrimap[]= "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/trimap3.jpg";
	char sOuput[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/AlphaMattingAlgorithm2.jpg";
	clock_t start, finish;
	start = clock();

	SharedMatting sm;
	sm.loadImage(sInput);
	sm.loadTrimap(sTrimap);
	sm.solveAlpha();
	sm.save(sOuput);

	finish = clock();
	cout << "Total Interval Seconds ( input3.jpg ) : ";
	cout << double(finish - start) / (CLOCKS_PER_SEC * 2.5) << endl;
}

void Sample4()
{
	// Todo: 你需要根据你的路径修改下此路劲
	char sInput[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/input4.jpg";
	char sTrimap[]= "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/trimap4.jpg";
	char sOuput[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/AlphaMattingAlgorithm2.jpg";
	clock_t start, finish;
	start = clock();

	SharedMatting sm;
	sm.loadImage(sInput);
	sm.loadTrimap(sTrimap);
	sm.solveAlpha();
	sm.save(sOuput);

	finish = clock();
	cout << "Total Interval Seconds ( input4.jpg ) : ";
	cout << double(finish - start) / (CLOCKS_PER_SEC * 2.5) << endl;
}

void Sample5()
{
	// Todo: 你需要根据你的路径修改下此路劲
	char sInput[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/input5.jpg";
	char sTrimap[]= "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/trimap5.jpg";
	char sOuput[] = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/AlphaMattingAlgorithm2.jpg";
	clock_t start, finish;
	start = clock();

	SharedMatting sm;
	sm.loadImage(sInput);
	sm.loadTrimap(sTrimap);
	sm.solveAlpha();
	sm.save(sOuput);

	finish = clock();
	cout << "Total Interval Seconds ( input5.jpg ) : ";
	cout << double(finish - start) / (CLOCKS_PER_SEC * 2.5) << endl;
}

int main()
{
	Sample1();
	Sample2();
	Sample3();
	Sample4();
	Sample5();
	

	int temp;
	cin >> temp;
	
	return 0;
}


