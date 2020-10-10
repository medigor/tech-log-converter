open System
open System.IO
open Imed.Parser
open System.Text
open System.Text.Json
open System.Diagnostics

let convert (file1, file2) =
    Console.Write(sprintf "%s   -->   %s" file1 file2)
    if File.Exists file2 then
        File.Delete file2
    use stream = File.OpenWrite file2
    
    let jwo = JsonWriterOptions(
                Encoder = Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Indented = false)
    

    use writer = new Utf8JsonWriter(stream, jwo)
    writer.WriteStartArray()
    
    Parser.getEvents file1
    |> Seq.iter (fun x ->
        JsonSerializer.Serialize(writer, x)
    )
    
    writer.WriteEndArray()
    writer.Flush()    
    Console.WriteLine("   complete")

[<EntryPoint>]
let main argv =
    if argv.Length <> 2 then
        Console.WriteLine("invalid args")
        1
    else if Directory.Exists(argv.[0]) |> not then
        Console.WriteLine("source dir not found")
        2
    else
        let dir1 = DirectoryInfo(argv.[0])
        let dir2 = DirectoryInfo(argv.[1])
        let w = Stopwatch.StartNew()
        let files =
            Directory.GetFiles(argv.[0], "*.log", SearchOption.AllDirectories)
            |> Array.map (fun x -> 
                let newDir = Path.Combine(dir2.FullName, Path.GetRelativePath(dir1.FullName, x))
                let newName = Path.ChangeExtension(newDir, "json")            
                x, newName 
            )
        
        files 
        |> Seq.map (fun (l,r) -> Path.GetDirectoryName r)
        |> Seq.distinct
        |> Seq.iter (Directory.CreateDirectory >> ignore)
        
        for x in files do
            convert x

        w.Stop()
        Console.WriteLine(sprintf "duration: %O" w.Elapsed)
        0