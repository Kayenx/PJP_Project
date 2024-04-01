grammar PJP_Project_expr;

program: statement+;

//Statements

statement:
    SEMICOLON #emptyCommand
    | primitiveType VARIABLE (COMMA VARIABLE)* SEMICOLON #declarationOfVariables
    | expression SEMICOLON #expressionEvaluation
    | READ VARIABLE (COMMA VARIABLE)* SEMICOLON #ReadFromInput
    | WRITE expression (COMMA expression)* SEMICOLON #WritetoOutput
    | '{' statement* '}' #BlockOfStatements
    | IF '(' expression ')' pos=statement (ELSE neg=statement)? #ConditionalStatement
    | WHILE '(' expression ')' statement #Cycle
    ;
//Expression
expression: 
    BOOL #boolean
    | INT #integer
    | FLOAT #float
    | STRING #string
    | VARIABLE #variable
    | '(' expression ')' #parentheses
    | prefix=MINUS expression #unaryMinus
    | prefix=LOGICNOT expression #logicNot
    | expression op=(MULT | DIV | MOD) expression #multiplyDivideModulo
    | expression op=(PLUS | MINUS | CONCAT) expression #plusMinusConcat
    | expression op=(LESSER | GREATER) expression #relation
    | expression op=(EQ | NEQ) expression #comparison
    | expression AND expression #logicAnd
    | expression OR expression #logicOR
    | <assoc=right> VARIABLE ASSIGN expression #assignment
    ;
primitiveType:
    type = INT_KEYWORD
    |type = FLOAT_KEYWORD
    |type = STRING_KEYWORD
    |type = BOOL_KEYWORD
    ;
INT_KEYWORD : 'int';
FLOAT_KEYWORD : 'float';
STRING_KEYWORD : 'string';
BOOL_KEYWORD : 'bool';

SEMICOLON: ';';
COMMA: ',';

LOGICNOT: '!';
MULT: '*';
DIV: '/';
MOD: '%';
PLUS: '+';
MINUS: '-';
CONCAT: '.';
LESSER: '<';
GREATER: '>';
EQ: '==';
NEQ: '!=';
AND: '&&';
OR: '||';
ASSIGN: '=';

READ : 'read' ;
WRITE : 'write' ;
IF : 'if' ;
ELSE : 'else' ;
WHILE : 'while' ;

VARIABLE :  [a-zA-Z] ([a-zA-Z0-9]*)? ;

//Literals
FLOAT : [0-9]+'.'[0-9]+ ;
INT : [0-9]+ ;
BOOL : ('true'|'false') ;
STRING : '"' (~["\\\r\n] | EscapeSequence)* '"';

fragment EscapeSequence
    : '\\' [btnfr"'\\]
    | '\\' ([0-3]? [0-7])? [0-7]
    ;

// SKIP

WS : [ \t\r\n]+ -> skip ;
COMMENT: '/*' .*? '*/' -> skip ;
LINE_COMMENT: '//' ~[\r\n]* -> skip ;