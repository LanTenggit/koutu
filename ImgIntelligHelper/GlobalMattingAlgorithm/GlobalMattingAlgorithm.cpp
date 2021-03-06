// GlobalMattingAlgorithm.cpp: 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include "globalmatting.h"
#include "guidedfilter.h"
#include "time.h"

#include <opencv2\opencv.hpp>

void Sample(cv::String index)
{
	// Todo: 你需要修改路径，否则运行不起来， You Need Modify Root Path
	cv::String sBasePath = "E:/WiseQ/WPF/private/ImgIntelligHelper/MattingImgs/";
	
	cv::String sInput = sBasePath + "input" + index + ".jpg";
	cv::String sTrimap = sBasePath + "trimap" + index + ".jpg";
	cv::String sOutput = sBasePath + "GlobalMatting_Result" + index + ".jpg";

	clock_t start, finish;
	start = clock();
	cv::Mat image = cv::imread(sInput, cv::ImreadModes::IMREAD_COLOR);
	cv::Mat trimap = cv::imread(sTrimap, cv::ImreadModes::IMREAD_GRAYSCALE);

	expansionOfKnownRegions(image, trimap, 9);

	cv::Mat foreground, alpha;
	globalMatting(image, trimap, foreground, alpha);

	// filter the result with fast guided filter
	alpha = guidedFilter(image, alpha, 10, 1e-5);
	for (int x = 0; x < trimap.cols; ++x)
		for (int y = 0; y < trimap.rows; ++y)
		{
			if (trimap.at<uchar>(y, x) == 0)
				alpha.at<uchar>(y, x) = 0;
			else if (trimap.at<uchar>(y, x) == 255)
				alpha.at<uchar>(y, x) = 255;
		}

	cv::imwrite(sOutput, alpha);
	finish = clock();
	cout << "Total Interval Seconds - File" <<  index << " : ";
	cout << double(finish - start) / (CLOCKS_PER_SEC * 2.5) << endl;
}

int main()
{
	// 挨个处理并生成图
	Sample("1");
	Sample("2");
	Sample("3");
	Sample("4");
	Sample("5");

	int temp;
	cin >> temp;
}


