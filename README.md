# Watching_Dog
看門狗程序,查看檔案有無修改過的痕跡。
當檔案有被修改,如改名、改內容等。
一開始會創建備份檔案夾,把東西都備份到那。還有一個changed的檔案夾,只要有檔案被改動就會從備份區那裏復原成原本的檔案,並把原先被改動的檔案丟到changed的檔案夾中。

GUI先開啟,並把你要監控的檔案夾路徑以及你的信箱輸入,按下send,當裡面的東西被改動會寄信給你。
注意GUI只能寄出一次,第二次寄出service就不會讀取了。除非你先把Watching Dog Service關掉,並重新開GUI才行。

[練習pipe用的來源網址](http://www.codebaoku.com/it-csharp/it-csharp-203094.html)
附註是使用visual studio2022開發。
