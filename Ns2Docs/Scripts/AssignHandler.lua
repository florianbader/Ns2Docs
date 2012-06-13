local SparkObjectHandler = require "SparkObjectHandler"
local Variable = Ns2Docs.Spark.Variable
local Table = Ns2Docs.Spark.Table
local VariableReference = Ns2Docs.Spark.VariableReference
local Library = Ns2Docs.Spark.Library

local AssignHandler = {}

function AssignHandler.Capture(offset, varList, expList, offsetEnd)
	local vars = varList:split("%s*,%s*")
	expList = expList or ""
	return 
	{
		__name="Assign", 
		vars=vars, 
		exps=expList, 
		line=line,
		offset=offset,
		offsetEnd=offsetEnd,
		Create = AssignHandler.Create
	}
end



function AssignHandler.Create(game, source, info, extraData)
	local docVars = {}
	local numVars = #info.vars
	local numExps = #info.exps

	for index=1, numVars do
		local varName = info.vars[index]
		local assignment
		if index <= numExps then
			assignment = info.exps[index]
		else
			assignment = info.exps[numExps]
		end
		
		local isTableConstructor = false
		if varName:endswith('Mixin') and assignment:match('%s*{%s*}%s*')then
			isTableConstructor = true
		end
		
		if not isTableConstructor then
			local variable = Variable(varName)
			variable.Assignment = assignment
			AssignHandler.SetUpVariable(game, source, variable, info, extraData)
			game:get_Variables():Add(variable)
			local refInfo = table.copy(info)
			refInfo.assignment = assignment
			AssignHandler.AddReference(game, source, variable, refInfo, extraData)
		else
			local sparkTable = game:FindTableWithName(varName)
			if not sparkTable then
				sparkTable = Table(varName)
				game:get_Tables():Add(sparkTable)
			end
			SparkObjectHandler.Init(game, source, sparkTable, info, extraData)
		end
	end
	return docVars

	--game:get_Variables():Add(Variable("someVar"))
end

function AssignHandler.SetUpVariable(game, source, variable, info, extraData)
	SparkObjectHandler.Init(game, source, variable, info, extraData)
end

function AssignHandler.AddReference(game, source, variable, info, extraData)
	local ref = VariableReference(info.assignment)
	
	local filePosition = source:GetFilePosition(info.offset)
	ref.DeclaredIn = source
	ref.Line = filePosition.Line
	ref.Column = filePosition.Column
	variable:get_References():Add(ref)
	
	if extraData.Library then
		if variable.Library ~= Library.Shared then
			if extraData.Library ~= variable.Library then
				variable.Library = Library.Shared
			end
		end
	end
end

return AssignHandler