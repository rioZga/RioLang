using Antlr4.Runtime;
using RioLang;
using RioLang.Content;

var fileName = "Content/test.rio"; //args[0]

var fileContents = File.ReadAllText(fileName);

AntlrInputStream inputStream = new AntlrInputStream(fileContents);
RioLexer rioLexer = new RioLexer(inputStream);
CommonTokenStream commonTokenStream = new CommonTokenStream(rioLexer);
RioParser rioParser = new RioParser(commonTokenStream);
RioParser.ProgramContext rioContext = rioParser.program();
RioVisitor visitor = new RioVisitor();
visitor.Visit(rioContext);