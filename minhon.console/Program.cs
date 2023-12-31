// See https://aka.ms/new-console-template for more information
using minhon;
using Refit;

Console.WriteLine("Hello, World!");

var client = RestService.For<IMachineTranslationClient>("https://mt-auto-minhon-mlt.ucri.jgn-x.jp");

var res = await client.Translate(Language.ja, Language.en, new TranslateRequest("test", "test", "こんにちは"));
