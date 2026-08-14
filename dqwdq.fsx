// ============================================================
// fileless_reflection.fsx
// Carrega uma DLL a partir de uma string Base64 (fileless)
// e usa reflexão para listar tipos e invocar um método.
// ============================================================

open System
open System.Reflection
open System.Text

// ------------------------------------------------------------
// 1. Coloque aqui a sua DLL codificada em Base64.
//    Para gerar: Convert.ToBase64String(File.ReadAllBytes("minha.dll"))
// ------------------------------------------------------------

// ------------------------------------------------------------
// 2. Decodifica e carrega o assembly em memória
// ------------------------------------------------------------
let assemblyBytes = Convert.FromBase64String(dllBase64)
let assembly = Assembly.Load(assemblyBytes)

printfn "Assembly carregado: %s" assembly.FullName

// ------------------------------------------------------------
// 3. Usa reflexão para listar todos os tipos públicos
// ------------------------------------------------------------
let tipos = assembly.GetExportedTypes()
printfn "\nTipos públicos encontrados:"
for t in tipos do
    printfn "  - %s" t.FullName

// ------------------------------------------------------------
// 4. Exemplo: localiza um tipo e invoca um método estático
//    (ajuste os nomes conforme sua DLL)
// ------------------------------------------------------------
let tipoAlvo = 
    tipos |> Array.tryFind (fun t -> t.Name = "MinhaClasse")  // substitua

match tipoAlvo with
| None -> printfn "\nTipo 'MinhaClasse' não encontrado."
| Some t ->
    printfn "\nTipo encontrado: %s" t.FullName

    // Lista métodos públicos (estáticos e de instância)
    let metodos = t.GetMethods(BindingFlags.Public ||| BindingFlags.Static ||| BindingFlags.Instance)
    printfn "Métodos:"
    for m in metodos do
        printfn "  - %s" m.Name

    // Tenta invocar um método estático chamado "Executar" (sem parâmetros)
    let metodo = t.GetMethod("Executar", BindingFlags.Public ||| BindingFlags.Static)
    match metodo with
    | null -> printfn "\nMétodo estático 'Executar' não encontrado."
    | m ->
        try
            let resultado = m.Invoke(null, null)  // null para estático, sem args
            printfn "\nResultado da invocação: %A" resultado
        with ex ->
            printfn "\nErro ao invocar: %s" ex.Message

    // Se quiser criar uma instância e invocar método de instância:
    // let instancia = Activator.CreateInstance(t)
    // let metodoInst = t.GetMethod("MetodoInstancia")
    // metodoInst.Invoke(instancia, [| |]) |> ignore

printfn "\nConcluído."
