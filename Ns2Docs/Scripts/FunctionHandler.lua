local SparkObjectHandler = require "SparkObjectHandler"
local Function = Ns2Docs.Spark.Function
local Parameter = Ns2Docs.Spark.Parameter
local Table = Ns2Docs.Spark.Table
local StaticFunction = Ns2Docs.Spark.StaticFunction
local Method = Ns2Docs.Spark.Method

local FunctionHandler = {}

function IsMethod(name, params)
	local methodNameStart = name:find(":[^:%.]+$")
	
    if methodNameStart then
        return true
	elseif params and (IsClassMethod(name, params) and params[1] == "self") then
        return true
    end
    return false
end

function IsClassMethod(name, params)
	local classMethodNameStart = name:find("%.[^:%.]+$")
    local isClassMethod = false
    if classMethodNameStart then        
		if params then
			if params[1] ~= "self" then
				isClassMethod = true
			end
		else
			isClassMethod = true
		end
		
	end
    return isClassMethod
end

local function CreateMethod(game, source, info, extraData)
	local methodnameStart = info.name:find("%:[^:%.]+$")
	local classname = info.name:sub(1, methodnameStart - 1)
	local methodname = info.name:sub(methodnameStart + 1)
	
	local class = game:FindTableWithName(classname)
	if class == nil then
		class = Table(classname)
		game:get_Tables():Add(class)
	end
	local method = Method(class, methodname)

	FunctionHandler.SetUpFunction(game, source, method, info, extraData)
	class:get_Methods():Add(method)
	
	for index, childInfo in ipairs(info.funcBody) do
		if childInfo.__name == 'Field' then
			local extraDataForChild = table.copy(extraData)
			extraDataForChild.declaringMethod = method
			extraDataForChild.declaringTable = class
			childInfo.Create(game, source, childInfo, extraDataForChild)
		end
	end
	
	return method
end

local function CreateClassMethod(game, source, info, extraData)
	local methodnameStart = info.name:find("%.[^:%.]+$")
	local classname = info.name:sub(1, methodnameStart - 1)
	local methodname = info.name:sub(methodnameStart + 1)
	
	local class = game:FindTableWithName(classname)
	if class == nil then
		class = Table(classname)
		game:get_Tables():Add(class)
	end
	local staticFunction = StaticFunction(class, methodname);
	FunctionHandler.SetUpFunction(game, source, staticFunction, info, extraData)
	class:get_StaticFunctions():Add(staticFunction)
	return staticFunction;
end

function FunctionHandler.Capture(...)
	local name, params, funcBody, offset, offset2
	local a = 0
	for _, value in ipairs(arg) do
		if type(value) == "table" then
			if value.object == "name" then
				name = value.content[1]
			elseif value.object == "params" then
				params = value.content[1]
				params = params:split("%s*,%s*")
			elseif value.object == "body" then
				funcBody = value.content
			end
		elseif type(value) == "number" then
			if not offset then
				offset = value
			else
				offsetEnd = value
			end
		end
	end
	
	params = params or {}
	
	local create
	
	if IsMethod(name or "", params) then
		create = CreateMethod
	elseif IsClassMethod(name or "", params) then
		create = CreateClassMethod
	else
		create = FunctionHandler.Create
	end

	return 
	{
		name = name, 
		params = params, 
		funcBody = funcBody, 
		offset = offset,
		offsetEnd = offsetEnd,
		Create = create
	}
end

function FunctionHandler.Create(game, source, info, extraData)
	local func = Function(info.name)
	FunctionHandler.SetUpFunction(game, source, func, info, extraData)
	SparkObjectHandler.Init(game, source, func, info, extraData)
	game:get_Functions():Add(func)
end

function FunctionHandler.SetUpFunction(game, source, func, info, extraData)
	for index, argName in ipairs(info.params) do
		func:GetOrCreateParameter(argName)
	end
	SparkObjectHandler.Init(game, source, func, info, extraData)
end

function FunctionHandler.CreateClassMethod(result, source, info, extraData)
	local methodnameStart = info.name:find("%.[^:%.]+$")
	local classname = info.name:sub(1, methodnameStart - 1)
	local methodname = info.name:sub(methodnameStart + 1)
	
	local class = CreateClass(result, classname, nil)	
	local method = CreateClassMethod(class, methodname, info.params)	
end

return FunctionHandler
