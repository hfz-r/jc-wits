   M S B A M L   `   `   `  ����  ESD.JC_Main"    ESD.JC_Main.Views.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   P LPrism.Wpf, Version=6.3.0.0, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59$prismhttp://prismlibrary.com/  5   	   !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  .  � Title$) #Warehouse Inventory Tracking System��5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � WindowState$ 	Maximized=�5   	   .  (System.Windows.Interactivity.Interaction   Triggers 5      F  @Prism.Interactivity.InteractionRequest.InteractionRequestTrigger  5   
   3  -System.Windows.Interactivity.EventTriggerBase   SourceObject 6*   �� *!ConfirmLogoutInteractionRequest+ �� Mode$ OneWay=�.  (System.Windows.Interactivity.TriggerBase   Actions. 5      +  %Prism.Interactivity.PopupWindowAction     IsModal . !	   CenterOverAssociatedObject	 . 65   5      .��5      � -   5        
 �Name$
 
LayoutRoot��5         MainBackground#��[  6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5    
   �� $��0��6   $��2��6"   $��2��63   .+�5!      �� $
��Left=�6   $
��Auto��63   $)��#Warehouse Inventory Tracking System��6@   $��Heavy"�6k   
���   �$��20(�6�   $��Cambria)�6�   $��Center=�6�   5"      �� 5$   
   $��1��6   $��2��6"   .+�5%      �� $��Right=�6   $
��Auto��64   $��Cambria)�6A   $��Center=�6V   $��0,0,10,0q�6q   �� 5&      $��SemiBold"�6=   
�������$��14(�6f   �6   �� *AuthenticatedUser+6f    �� 5'      $� ��6   $��	ExtraBold"�6#   
����  �6:    �� 5(      $��0��6    ^� ToolTip$ Click here to logout��6+   ��6Y   �� *LogoutCommand+��5)      ��  �� RelativeSource ��  �� AncestorType" ���5*      �� 5+       �� 
TargetType" ��6.   .R�5,   "   �� 5-      .��5/      �� -   5         ��Name$ 	logoutTxt��5/      $�(logout)��61   $��Normal"�6A   
���  ��$��12(�6f   50      51      52      �� 54   
   $��0��6   $��2��6!   
�������$	��250��6E   $��5,0,0,5q�6T   ._�55      �� -   5         ��Name$ NavigationItemsControl��55      !  Prism.Regions.RegionManager   
RegionName$ MainNavigationRegion��6;   $��0��6q   $��5q�6�   $��5q�6�   56      �� 57   
   $ MainContentRegion��6   $��1��58      $��2��6)   $��5,0,5,5q�66   $��Stretch=�6G   $��Stretch=�6l   5:      5;      