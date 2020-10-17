
#pragma once
#include "stdafx.h"
#include <opencv2/opencv.hpp>

using namespace cv;
using namespace std;

void expansionOfKnownRegions(InputArray img, InputOutputArray trimap, int niter = 9);
void globalMatting(InputArray image, InputArray trimap, OutputArray foreground, OutputArray alpha, OutputArray conf = noArray());