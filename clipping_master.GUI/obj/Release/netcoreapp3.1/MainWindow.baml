   M S B A M L   `   `   `  ���� J  FLibVLCSharp.WPF, Version=3.6.6.0, Culture=neutral, PublicKeyToken=nullJ6clr-namespace:LibVLCSharp.WPF;assembly=LibVLCSharp.WPFLibVLCSharp.WPF  N Jclipping_master.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:ffmpeg_helper.GUIffmpeg_helper.GUI "   ffmpeg_helper.GUI.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Fvlc6clr-namespace:LibVLCSharp.WPF;assembly=LibVLCSharp.WPF     1localclr-namespace:ffmpeg_helper.GUI     -     � Title$  Clipping Master��$	��600��$	��600��   CustomWindowStyle#���   ���� *WindowWidth+ �� Mode$ TwoWay=� � DataContext )  #ffmpeg_helper.GUI.WindowDataContext  .���  � RowDefinitions �� $	��350 ��� $��40 ��� $��40 ��� $��30 ��� $��30 ��� $��* �.+�   LibVLCSharp.WPF.VideoView  -     Name$ BackgroundVideo��$��0��$��0 20 0 0q� �� EdgeMode$ Aliased=�� $��1�� � ColumnDefinitions �� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� -   $��MarkStartButton��$��0��$
��5, 5q�$��
Mark Start���� -   $��
PlayButton��$��1��$
��5, 5q�$
��Play���� -   $��PauseButton��$��2��$
��5, 5q�$��Pause���� -   $��
StopButton��$��3��$
��5, 5q�$
��Stop���� -   $��MarkEndButton��$��4��$
��5, 5q�$��Mark End��� $��2�� �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� -   $��DecreaseMin��$��0��$
��5, 5q�$
��-min���� -	   $��DecreaseSec��$��1��$
��5, 5q�$��-second���� -
   $��DecreaseTick��$��2��$
��5, 5q�$��-tick���� -   $��IncreaseTick��$��3��$
��5, 5q�$��+tick���� -   $��IncreaseSec��$��4��$
��5, 5q�$��+second���� -   $��IncreaseMin��$��5��$
��5, 5q�$
��+min���� -   $��TimeSplider��$��3��$��Center=�$��30, 0q� � Minimum$ 0Q� � Maximum �� *
Duration+	 � Value	 �� *TimePosition+$ TwoWay=��� $��3��$��1q�
���\\������ *RangeMargin+y��� *RangeVisibility+�� $��4��$��Center=�$��18(�$��Center=�$��30, 0q����� *
TimeCode+� $��5�� �� $��* ��� $��* �.+��� -   $��ClipList��$��0��$��10q�
 � ContextMenu
 �� .@�m� -   $��
AddNewClip��$��Add New��m� -   $��CleanAllClip��$��Clean���� $��1��$��10q� �� Orientation$ Vertical=�$��Visible=�y��� *ContentVisibility+.+��  �� $��75 ��� $��25 ��� $��* � �� $��30 ��� $��30 �.+��� $��0��$��0��$��Start��$��Center=��� $��1��$��0��$	��End��$��Center=��� -   $��	GoToStart��$��0��$��1��$��Go���� -   $��GoToEnd��$��1��$��1��$��Go���� -   $��StartVar��$��0��$��2��${�False��$��Center=�$��10, 0q����� *SelectRange.StartString+�� -   $��EndVar��$��1��$��2��${�False��$��Center=�$��10, 0q����� *SelectRange.EndString+o� .@�m� $
��File��$
��15 3q�.@�m� -   $��	OpenVideo��$��
Open Video��$
��15 3q�m� -   $��Export��$��Export��$
��15 3q�m� -   $
��Exit��$
��Exit��$
��15 3q�