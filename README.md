# Watching_Dog
看門狗程序,查看檔案有無修改過的痕跡。 </br>
當檔案有被修改,如改名、改內容等。</br>
一開始會創建備份檔案夾,把東西都備份到那。</br>
還有一個changed的檔案夾,只要有檔案被改動就會從備份區那裏復原成原本的檔案,並把原先被改動的檔案丟到changed的檔案夾中。</br>

GUI先開啟,並把你要監控的檔案夾路徑以及你的信箱輸入,按下send,當裡面的東西被改動時,系統會寄信給你。這個系統的gmail和密碼你要在程式碼關於寄信的部分去做修改,可能用一個你不需要的信箱來用,這樣才能完成A信箱(系統)傳到B信箱(你現在輸入在GUI的gmail)。</br>
注意系統的信箱密碼不是單純的gmail密碼,而是"應用程式密碼",去帳戶的設定裡面去申請即可。</br>
注意GUI只能寄出一次,第二次寄出service就不會讀取了。除非你先把Watching Dog Service關掉,並重新開GUI才行。</br>

3.0的部分</br>
在cmd打這指令會安裝服務</br>

~~~
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe   Watching_Dog.exe
~~~

解安裝
~~~
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u Watching_Dog.exe
~~~

注意要先cd到Watching_Dog.exe的路徑才行,不然就是執行檔得放在c槽user檔案夾中,還有有時候不一定會是v4.0.30319這版本的,所以去C:\WINDOWS\Microsoft.NET\Framework 看看各種版本中的檔案夾有無InstallUtil.exe,找有的並把指令換成你的版本

[練習pipe用的來源網址](http://www.codebaoku.com/it-csharp/it-csharp-203094.html)
附註是使用visual studio2022開發。
