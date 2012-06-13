local lpeg = require 'lpeg'
local grammar = require 'leg.grammar'
local parser = require 'leg.parser'
local scanner = require 'leg.scanner'

require 'Utils'
local ClassHandler = require 'ClassHandler'
local FunctionHandler = require 'FunctionHandler'
local IfHandler = require 'IfHandler'
local AssignHandler = require 'AssignHandler'
local AssignExpListHandler = require 'AssignExpListHandler'
local StaticFieldHandler = require 'StaticFieldHandler'
local FieldHandler = require 'FieldHandler'
local PrepareMixinHandler = require 'PrepareMixinHandler'
local DislocatedCommentHandler = require 'DislocatedCommentHandler'

local Utils = Ns2Docs.Utils

local ParseResult = Ns2Docs.ParseResult
local ParseException = Ns2Docs.ParseException

local pattern = nil

function ParseString(game, source)
	
	local matches = pattern:match(source.Contents)
	extraData = {Library = source.Library}
	for _, match in ipairs(matches) do
		CreatingObject(match.__name or 'Unnamed Type')
		local sparkObject = match.Create(game, source, match, table.copy(extraData))
    end
end



function MakeRules()
	local doc_comment = #(lpeg.P'/**') * function (subject, i1)
        local level = _G.assert( subject:match('^/%*%*', i1) )
        local _, i2 = subject:find('*/', i1+3, true)  -- true = plain 'find substring'
        return (i2 and (i2+1)) or error('unfinished doc comment')(subject, i1)
    end
	
	local dislocated_comment = #(lpeg.P'/*#') * function (subject, i1)
        local level = _G.assert( subject:match('^/%*#', i1) )
        local _, i2 = subject:find('*/', i1+3, true)  -- true = plain 'find substring'
        return (i2 and (i2+1)) or error('unfinished dislocated comment')(subject, i1)
    end

	customLegRules = table.copy(parser.rules)
	
	customLegRules.Function = lpeg.Cp() * customLegRules.Function * lpeg.Cp()
	customLegRules.GlobalFunction = lpeg.Cp() * customLegRules.GlobalFunction * lpeg.Cp()
	customLegRules.Class = lpeg.Cp() * customLegRules.Class * lpeg.Cp()
	customLegRules.PrepareMixin = lpeg.Cp() * customLegRules.PrepareMixin * lpeg.Cp()
	
	
    local S = scanner.SPACE ^ 0
    local rules = {
        (
			((lpeg.V'DOC_COMMENT' * S)^0) * S * (
				lpeg.V'Function' + 
				lpeg.V'GlobalFunction' + 
				lpeg.V'Class' + 
				lpeg.V'PrepareMixin' + 
				lpeg.V'CustomAssign' + 
				lpeg.V'If' + 
				lpeg.V'FieldAssign' +
				lpeg.V'StaticFieldAssign'
			)
			+
			(lpeg.V'DISLOCATED_COMMENT_WITH_POS')
		),
        DOC_COMMENT = doc_comment,
		DISLOCATED_COMMENT = dislocated_comment,
		DISLOCATED_COMMENT_WITH_POS = lpeg.Cp() * lpeg.V'DISLOCATED_COMMENT',
		AssignExp = parser.rules.Exp,
		AssignExpList = grammar.listOf(lpeg.V'AssignExp', S* lpeg.V',' *S),
		FieldAssignExp = lpeg.V'AssignExp',
		CustomAssign = lpeg.Cp() * lpeg.V'NameList' *lpeg.V'IGNORED'* lpeg.V'=' *lpeg.V'IGNORED'* lpeg.V'AssignExpList' * lpeg.Cp(),
		FieldAssign = lpeg.Cp() * 'self.' * lpeg.V'SuperClass' *S* '=' *S* lpeg.V'FieldAssignExp' * lpeg.Cp(),
		StaticFieldAssign = lpeg.Cp() * lpeg.V'SuperClass' * '.' * lpeg.V'Var' *S* '=' *S* lpeg.V'FieldAssignExp' * lpeg.Cp(),
		Stat = lpeg.V'CustomAssign' + lpeg.V'FieldAssign' + customLegRules.Stat
    }
	
	return rules
end

function MakeCaptures()
	local S = lpeg.V'IGNORED'
	expList = {
		captures = 
		{
			function(...)
				return arg
			end,
			Exp = function(...)
				return nil
			end
		},
		rules = 
		{
			grammar.listOf(lpeg.V'CustomExp' , S* lpeg.V',' *S),
			CustomExp = lpeg.Cp() * lpeg.V'Exp' * lpeg.Cp()
		}
	}
    expList.pattern = lpeg.P( grammar.apply(parser.rules, expList.rules, expList.captures) )
    local captures = {
		function (...)  -- the initial rule
			local tbl = {}
			for _, value in ipairs(arg) do
				tbl = table.combine(tbl, value)
			end
            return tbl
		end,

		Function = FunctionHandler.Capture,
		Class = ClassHandler.Capture,
		CustomAssign = AssignHandler.Capture,
		NameList = grammar.C,
		VarList = grammar.C,
		AssignExpList = AssignExpListHandler.Capture,
		FieldAssignExp = grammar.C,
		ClassName = scanner.string2text,
		SuperClass = grammar.C,
		PrepareMixin = PrepareMixinHandler.Capture,
		MixinID = grammar.C,
		TableConstructor = grammar.C,
		StaticFieldAssign = StaticFieldHandler.Capture,
		FieldAssign = FieldHandler.Capture,
		Var = grammar.C,
        FuncName  = function(...)
			return {object="name", content=grammar.C({...})}
        end,
		 
        ParList  = function(...)    
			return {object="params", content=grammar.C({...})}
		end,
		

        Block = function(...)    
			return {object="body", content={...}}
		end,
		
		If = IfHandler.Capture,
		
		Exp = grammar.C,
		
		AssignExp = function() end,
		Assign = function() end,
			
        DOC_COMMENT = function(comment)
            return {comment=Utils.CleanUpComment(comment)}
        end,
		
		DISLOCATED_COMMENT = function(comment)
			return Utils.CleanUpComment(comment)
		end,
		
		DISLOCATED_COMMENT_WITH_POS = DislocatedCommentHandler.Capture,
    }   

	captures.GlobalFunction = captures.Function
	return captures
end

function MakePattern(rules, captures)
	pattern = lpeg.P( grammar.apply(customLegRules, rules, captures) )
    local Stat = lpeg.P( grammar.apply(customLegRules, lpeg.V'Stat', nil) )
    pattern = (pattern + Stat + scanner.ANY)^0 / function(...) 
		local tbl = {}
		for _, value in ipairs(arg) do
			if type(value) == "table" then
				table.insert(tbl, value)
			end
		end
        return tbl
    end
	
    return pattern
end
