   M S B A M L   `   `   `  ����K  GAlphaMattingDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:AlphaMattingDemoAlphaMattingDemo  5   	   !    AlphaMattingDemo.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:AlphaMattingDemo     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      � 
������� � RowDefinitions 5
   
   �� 5      $��40 �6   �� 5      �� 5      $��40 �6   �� 5      $��40 �6   �� 5      $��40 �6    � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      .+�5   
   �� $��Center=�6   $��原图��61   $��Center=�6;   $��24(�5      �� 5   
   $��Center=�6   $��	Trimap图��61   $��Center=�6@   $��24(�5      $��1��6"   �� 5   
   $��Center=�6   $��结果呈现��61   $��Center=�6=   $��24(�5      $��2��6"   �� 5   
   -   5         ��Name$ 	ImgOrigin��5      $��1��6#   $��5q�60   �� 5   
   -   5        $ 
ImgMatting��5      $��1��6$   $��1��61   $��5q�6A   �� 5   
   -   5        $ 	ImgResult��5      $��1��6$   $��2��61   $��5q�6A   �� 5   
   $��Center=�6   $��Center=�62   $��3��5       $��2��6)    �� Orientation$ 
Horizontal=�66   .+�5!      �� -   5         ��Name$ 
BtnSample1��5!      $��
图1测试��6)   $��80��68   $��32��6C   $��18(�5"      $��0,1,0,0q�6#   65   �� 5#      -   5        $ 
BtnSample2��5#      $��
图2测试��6)   $��80��68   $��32��6C   $	��5,0q�6O   $��18(�5$      $��0,1,0,0q�6#   65   �� 5%      -   5        $ 
BtnSample3��5%      $��
图3测试��6)   $��80��68   $��32��6C   $	��5,0q�6O   $��18(�5&      $��0,1,0,0q�6#   65   �� 5'      -   5        $ 
BtnSample4��5'      $��
图4测试��6)   $��80��68   $��32��6C   $	��5,0q�6O   $��18(�5(      $��0,1,0,0q�6#   65   �� 5)      -   5        $ 
BtnSample5��5)      $��
图5测试��6)   $��80��68   $��32��6C   $	��5,0q�6O   $��18(�5*      $��0,1,0,0q�6#   65   5+      �� 5,   
   -	   5         ��Name$ TbkStartTime��5,      $��Center=�6*   $��16(�5-      $��Center=�6"   $��3��6?   �� 5.   
   -
   5        $ 
TbkEndTime��5.      $��Center=�6(   $��1��6C   $��3��6S   $��16(�5/      $��Center=�6"   �� 50   
   -   5        $ 
TbkSeconds��50      $��Center=�6(   $��2��6C   $��3��6S   $��30(�51      $��Center=�6"   �� 52   
   -   5        $ TbkPixelInfo��52      $��Center=�6*   $��3��6E   $��4��6Y   $��24(�53      $��Center=�6"   �� 54   
   -   5        $��测试��54      $��2��6   $
��Left=�6.   $��	126,4,0,0q�6I   $��2��6\   $	��Top=�6i   $��92��6�   $��32��6�   6�   55      56      