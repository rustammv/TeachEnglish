   M S B A M L   `   `   `  ����H  DRuma.Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullD2clr-namespace:Ruma.Controls;assembly=Ruma.ControlsRuma.Controls  5   	    TeachEnglish.Views!   TeachEnglish.Views.AboutBox   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Dlocal2clr-namespace:Ruma.Controls;assembly=Ruma.Controls     5   	     � ShowInTaskbar  .   � 
ResizeMode$ NoResize=�5       � SizeToContent$ Height=�65    � Title$ AboutBox��5   	   $	��301��6   $	��445��6'    � WindowStartupLocation$ CenterScreen=�63      VS2012WindowStyle#���   6X    � 	Resources 5       �� MergedDictionaries �� Source�� 5	   
    5
      �� 5      $, &Resource/VS2012/VS2012WindowStyle.xaml?�6%   �� 5      $ Resource/Label.xaml?�6%   �� 5      $ Resource/ScrollBar.xaml?�6%   �� 5      $ Resource/TextBox.xaml?�6%   %    5      .��5      � ���	#FF2D2D30.+�5   
   �� 
clientArea��.+�5      �� displayArea��$��10q�6-   $	��240��69   .+�5      �� -   5         ��Name$ productName��5      productName��$��25��6A   �� 5      versionArea��	 �� Orientation$	 
Horizontal=�61   .+�5      �� -   5        $ versionLabel��5      $��25��62   VersionLabel��$��
Version - ��6S   �� 5      -   5        $ version��5      $��25��6-   version��69   5      �� 5       -   5        $ 	copyright��5       $��25��6+   	copyright����6I   �� 
 �� Mode$
 OneTime=� �� XPath$ 	Copyright���� 5!      -   5        $ company��5!      $��25��6)   company����6E   �� $
 OneTime=�$ Company���� 5"      -   5        $ reserved��5"      $��25��6*   reserved��$��All Rights Reserved.��6G   �� 5#      -   5        $
 info��5#      $��25��6&   info��$��Product details:��6?   ���#5556Z   �� 5$      -   5         ��Name$ description��5$      description��" �� VerticalScrollBarVisibility$
 Auto=�6t   $��50��6�   ��6C   �� $
 OneTime=�$ Description��6�   ]� 5%      
buttonArea��.+�5&      �� -	   5        $ productLink��5&   M   $
��Left=�6   $
��Left=�62   productLink��5-         Ruma.Controls.ButtonText  5.      -
   5          Name$ okButton��5.   {    �� 	IsDefault .  �� IsCancel . $��Right=�6H   $��Right=�6_   okButton��$��OK��6�   $��85��6�   5/      50      51      52      53      