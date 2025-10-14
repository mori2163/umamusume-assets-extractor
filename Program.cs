using Microsoft.Data.Sqlite;
using System.Reflection.PortableExecutable;
using System.Threading;
using static Umamusume_Assets_Extractor.Utils;

// 起動
UpdateConsoleTitle();
Console.WriteLine(appName + " v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
Console.WriteLine("自分が見たいファイルがなかった場合はウマ娘で一括ダウンロードをしてください。");
Console.WriteLine($"ダンプしたファイルは{extractFolderName}というフォルダーに保存されます。");
Console.WriteLine();

// ゲームデータパスの確認と手動設定
if (string.IsNullOrEmpty(gameDataPath))
{
    Console.WriteLine("エラー:ゲームデータのパスが自動で見つかりませんでした。");
    Console.WriteLine();
    
    bool pathSet = false;
    while (!pathSet)
    {
        Console.WriteLine("ゲームデータが保存されているフォルダーのパスを手動で入力してください。");
        Console.WriteLine("(例: C:\\Program Files (x86)\\Steam\\steamapps\\common\\UmamusumePrettyDerby_Jpn\\UmamusumePrettyDerby_Jpn_Data\\Persistent)");
        Console.Write("パス:");
        string customPath = Console.ReadLine();
        
        if (string.IsNullOrEmpty(customPath))
        {
            Console.WriteLine("エラー:パスが入力されていません。もう一度入力してください。");
            Console.WriteLine();
            continue;
        }
        
        if (SetCustomGameDataPath(customPath))
        {
            pathSet = true;
            Console.WriteLine($"パスが設定されました: {gameDataPath}");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("もう一度入力してください。");
            Console.WriteLine();
        }
    }
}

// ログを表示するかの確認
Console.WriteLine("コンソールにログを表示しますか？");
Console.WriteLine("表示すると、実行速度が少し遅くなります。速度を求める場合は表示しないことをお勧めします。表示しなくてもタイトルバーで進捗を確認できます。");
Console.Write("表示する場合はyを、しない場合は他のキーを入力してEnterを押してください:");
verboseMode = Console.ReadLine() == "y";

// ダンプする対象がファイルかフォルダかの確認
Console.WriteLine();
Console.WriteLine();
Console.WriteLine("ファイルとフォルダーどちらをダンプしますか？");
Console.WriteLine("ファイルをダンプする場合、入力された文字が名前に入っているファイルがすべてダンプされます。");
Console.WriteLine("フォルダーをダンプする場合、入力された文字の名前のフォルダーをダンプします。");
Console.Write("ファイルをダンプする場合はyを、フォルダーをダンプする場合はそれ以外の文字を入力してEnterを押してください:");
isDumpTargetFile = Console.ReadLine() == "y";

var dumpTargetName = isDumpTargetFile ? "ファイル" : "フォルダー";

// ダンプするフォルダーを指定する
var dumpTarget = "";
Console.WriteLine();
Console.WriteLine();
do
{
    Console.WriteLine($"ダンプする{dumpTargetName}を指定してください。指定しない場合は空欄にするとすべての{dumpTargetName}をダンプします。");
    Console.WriteLine(isDumpTargetFile ? "例:1001と入力すると1001が名前に含まれているファイルがすべてダンプされます。" :
                                         "例:soundと入力するとacbファイルとawbファイルが入っているsoundフォルダーのみダンプします。");
    Console.WriteLine("フォルダー一覧を表示したい場合はlistと入力してください。");
    Console.Write($"{dumpTargetName}名を入力:");
    dumpTarget = Console.ReadLine();

    if (dumpTarget == "list")
        PrintFolders();
} while (dumpTarget == "list");

if (dumpTarget == null)
{
    dumpTarget = "";
}

if (!Directory.Exists(extractFolderName))
{
    PrintLogIfVerboseModeIsOn($"フォルダー{extractFolderName}を作成中");
    Directory.CreateDirectory(extractFolderName);
}

// ファイルをコピー
CopySourceFiles(dumpTarget);
UpdateConsoleTitle("done");

Console.WriteLine("展開したフォルダーをエクスプローラーで開きますか？(y)");
if (Console.ReadLine() == "y")
    System.Diagnostics.Process.Start("explorer.exe", extractFolderName);