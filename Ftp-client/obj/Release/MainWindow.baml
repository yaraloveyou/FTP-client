   M S B A M L   `   `   `  ���� E  AFtp-client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Ftp_client
Ftp_client      Ftp_client.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     *localclr-namespace:Ftp_client     a ]GongSolutions.WPF.DragDrop, Version=2.0.0.0, Culture=neutral, PublicKeyToken=91f1945125b7a587ddurn:gong-wpf-dragdrop    � Title$  FTP-клиент��$	��707��$	��800��$��Lucida Console)� � WindowState$ 	Maximized=� � WindowStartupLocation$ CenterScreen=�.��� -   $��mainGrid�� � ColumnDefinitions �� $
��390* ��� $��3 ��� $
��397* � � RowDefinitions �� $
��263* ��� $��3 ��� $
��296* ��� $��3 ��� $	��111 �.+�o� $��25��$	��Top=�$��3��.@�.@�
Хост: �� -    ��Name$
 host��$�� ��$
��95px�� �� 	MaxLength$ 15��$" Имя пользователя: �� -   $ login��$
��95px��$ 16��$�� �� Пароль: G� -    G�Name$ password��$
��95px�� G� 	MaxLength$ 32��	 G� Password$	  �� Порт: �� -   $
 port��$�� ��$
��40px��$ 5���� -   
 ��Name$
 connect��$��100px��.��Соединение�� -   $ status�� �� 
IsReadOnly . $��3��$��0,25,0,0q�" �� VerticalScrollBarVisibility$
 Auto=�$��2��R� -    R�Name$ treeOnPC��$��2��$��0��$��3��$��0,25,0,0q��� -	    ��Name$ listViewOnServer��$��12(�$��2��$��1��
���   �$��2��$��	0,25,0,20q�)  #GongSolutions.Wpf.DragDrop.DragDrop   IsDragSource .    IsDropTarget .  � ContextMenu �� -
   .@�m� -   $��contextServerDownload��$��Скачать��m� -   $��contextServerAddInTask��$1��+Добавить файл в задание��m� -   $ ��contextServerInputInFolder��$"��Войти в каталог���� m� -   $��contextServerCreateFolder��$#��Создать каталог��m� -   $��contextServerCreateFile��$(��"Создать новый файл��m� -   $��contextServerRefresh��$��Обновнить���� m� -   $��contextServerDelete��$��Удалить��m� -   $��contextServerCopyPath��$@��:Копировать адрес в буфер обмена������  �� 
TargetType" ��.�-    �� View �� P���  �� Width$	 150��$��Имя файла�� �� CellTemplate �� .R���  �� Orientation$ 
Horizontal=�.+��� $��Uniform=�$��15������ *ImageSource+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=��� $��12(�$��0,0,0,0q�����  �� Path$
 Name�$ PropertyChanged=�$ TwoWay=��� $	 100��$��Размер�� �� DisplayMemberBinding �� $
 Size�$ PropertyChanged=�$ TwoWay=��� $	 100��$��
Права�� �� $
 Role�$ PropertyChanged=�$ TwoWay=�.@��� $	��100��$	��100��.@��� .���� $��Размер���� -   $ listViewOnPC��$��12(�$��2��
���   �$��	0,25,0,20q� .  .  �� -   .@�m� -   $��contextPCDownload��$��Скачать��m� -   $��contextPCAddInTask��$1��+Добавить файл в задание��m� -   $��contextPCInputInFolder��$"��Войти в каталог���� m� -   $��contextPCOpen��$��
Войти���� m� -   $��contextPCCreateFolder��$#��Создать каталог��m� -   $��contextPCCreateFile��$(��"Создать новый файл��m� -   $��contextPCRefresh��$��Обновнить���� m� -   $��contextPCDelete��$��Удалить��m� -   $��contextPCCopyPath��$@��:Копировать адрес в буфер обмена������ " ��.�-    �� P��� $��Имя файла�� �� .R��� $ 
Horizontal=�.+��� $��Uniform=�$��15������ *ImageSource+$ PropertyChanged=�$ TwoWay=��� $��12(�$��0,0,0,0q����� $
 Name�$ PropertyChanged=�$ TwoWay=��� $	 100��$��Размер�� �� $
 Size�$ PropertyChanged=�$ TwoWay=��� $ 80��$��Тип файла�� �� $
 Type�$ PropertyChanged=�$ TwoWay=��� $+��%Последнее изменение�� �� $ 
LastChange�$ PropertyChanged=�$ TwoWay=��� -    $��
statusOnPC��$��13(� �� Padding$ 0,4,0,0q�$��Размер: ��$��2��
�������$��20��$��Bottom=��� -!   $��statusOnServer��$��13(�$ 4,4,0,0q�$!��Нет соединения��$��2��
�������$��2��$��20��$��Bottom=��� -"   $��txtPC��$��2��$	��C:/�� �� MaxLines$ 1��$��0,5,0,0q�$��	120,0,0,0q�$��25��$	��Top=��� $��2��
�������$"��Локальный сайт:��$ 4,6,0,0q�$
��Left=�$	��120��$��25��$	��Top=��� -#   $��	txtServer��$��2��$��/��$��2��$ 1��$��0,5,0,0q�$��	115,0,0,0q�$��25��$	��Top=��� $��2��
�������$"��Удаленный сайт:��$��2��$ 4,6,0,0q�$
��Left=�$	��117��$��25��$	��Top=��� $��4��$��3�� �� TabStripPlacement$ Bottom=�.@��� $"��Файлы в задании��.��� ���	#FFE5E5E5.+��� -$   $��
listInTask��$��12(�
���   � �� -%   .@�m� -&   $��execute��$'��!Выполнить задание��m� -'   $��	deleteAll��$��Удалить все��m� -(   $��deleteSpecific��$'��!Удалить выбранное�� �� P��� $	 150��$!��Локальный файл�� �� .R��� $ 
Horizontal=�.+��� $��Uniform=�$��15������ *ImageSource+$ PropertyChanged=�$ TwoWay=��� $��12(�$��0,0,0,0q����� $ PathOnPC�$ PropertyChanged=�$ TwoWay=��� $	 100��$��Направление�� �� $ SideTransmission�$ PropertyChanged=�$ TwoWay=��� $	 200��$"��Файл на сервере�� �� $ PathOnServer�$ PropertyChanged=�$ TwoWay=��� $	 100��$��Размер�� �� $
 Size�$ PropertyChanged=�$ TwoWay=��� $-��'Неудавшиеся передачи��.��� ���	#FFE5E5E5.+��� -)   $��listTransferFailed��$��12(�
���   � �� -*   .@�m� -+   $��deleteAllTransferFailed��$��Удалить все��m� -,   $"��deleteSpecificTransferFailed��$'��!Удалить выбранное�� �� P��� $	 150��$!��Локальный файл�� �� .R��� $ 
Horizontal=�.+��� $��Uniform=�$��15������ *ImageSource+$ PropertyChanged=�$ TwoWay=��� $��12(�$��0,0,0,0q����� $ PathOnPC�$ PropertyChanged=�$ TwoWay=��� $	 100��$��Направление�� �� $ SideTransmission�$ PropertyChanged=�$ TwoWay=��� $	 200��$"��Файл на сервере�� �� $ PathOnServer�$ PropertyChanged=�$ TwoWay=��� $	 100��$��Размер�� �� $
 Size�$ PropertyChanged=�$ TwoWay=��� $	 200��$��
Время�� �� $
 Time�$ PropertyChanged=�$ TwoWay=��� $'��!Успешные передачи��.��� ���	#FFE5E5E5.+��� --   $��listTransferSuccess��$��12(�
���   � �� -.   .@�m� -/   $��deleteAllTransferSuccess��$��Удалить все��m� -0   $#��deleteSpecificTransferSuccess��$'��!Удалить выбранное�� �� P��� $	 150��$!��Локальный файл�� �� .R��� $ 
Horizontal=�.+��� $��Uniform=�$��15������ *ImageSource+$ PropertyChanged=�$ TwoWay=��� $��12(�$��0,0,0,0q����� $ PathOnPC�$ PropertyChanged=�$ TwoWay=��� $	 100��$��Направление�� �� $ SideTransmission�$ PropertyChanged=�$ TwoWay=��� $	 200��$"��Файл на сервере�� �� $ PathOnServer�$ PropertyChanged=�$ TwoWay=��� $	 100��$��Размер�� �� $
 Size�$ PropertyChanged=�$ TwoWay=��� $	 200��$��
Время�� �� $
 Time�$ PropertyChanged=�$ TwoWay=��� -1    ��Name$ 	splitter1��
�������$��1��$��2�� �� ResizeDirection$ Columns=�$��Stretch=��� -2   $ 	splitter2��
�������$
 Rows=�$��Stretch=�$��1��$��3���� -3   $ 	splitter3��
�������$
 Rows=�$��Stretch=�$��3��$��3��