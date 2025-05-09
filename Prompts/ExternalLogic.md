# Create an External Logic
This is for my Developer Day Tokyo 2025 session.
The tool I used this agains was GitHub Copiot.

see: [登壇記録：Developer Day Tokyo 2025](https://blog.house-soft.info/archives/2269#%E3%82%B9%E3%83%A9%E3%82%A4%E3%83%89%E4%B8%AD%E3%81%A7%E4%BD%BF%E3%81%A3%E3%81%9F%E3%83%97%E3%83%AD%E3%83%B3%E3%83%97%E3%83%88%E4%BE%8B)

```
次の条件を満たすC#のメソッドを作成してください。
前提
– .NET 8
– OSはLinux
– ソリューション及びプロジェクトの作成は不要
– NuGetにあるライブラリを使って良い（必要なNuGetライブラリについては、VSCode上でプロジェクトに追加するためのコマンドリストを生成すること）

– ただし、無償で使えるライブラリに限定
– メソッドはinterfaceで定義し、そのinterfaceを実装する
– interfaceとclassは別ファイルで定義する
– interfaceには「OSInterface」属性を付与する
– interface内のメソッドには「OSAction」属性を付与する
– OSInterface及びOSAction属性は別の場所で定義したものを参照するので、定義部分は出力しない

仕様
- 
```

## How to use
Write specifications for Actions you want to create under "仕様" as a list.
ex:
```
仕様
– Excelの指定セルに値を書き込むメソッド
Excelのバイナリファイル、シート位置、セル位置、値を渡し、該当セルに値を書き込んだ結果のExcelバイナリファイルを返す
– Excelの指定セルから値を読み込むメソッド
Excelのバイナリファイル、シート位置、セル位置を渡し、該当セルから値を読み、文字列として返す。
```
