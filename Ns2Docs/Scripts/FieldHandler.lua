local AssignHandler = require "AssignHandler"
local Field = Ns2Docs.Spark.Field
local Table = Ns2Docs.Spark.Table
local SparkObjectHandler = require "SparkObjectHandler"

local FieldHandler = {}

function FieldHandler.Capture(offset, fieldName, assignment, offsetEnd)
	
	return 
	{
		__name='Field',
		fieldName=fieldName,
		assignment=assignment,
		line=line,
		offset=offset,
		offsetEnd=offsetEnd,
		Create=FieldHandler.Create
	}
end



function FieldHandler.Create(game, source, info, extraData)
	local sparkTbl = extraData.declaringTable
	local fieldName = info.fieldName
	local field = sparkTbl:FindFieldWithName(fieldName)
	if not field then
		field = Field(sparkTbl, fieldName)
		field:set_Assignment(info.assignment)
		AssignHandler.SetUpVariable(game, source, field, info, extraData)
		sparkTbl:get_Fields():Add(field)
	end
	AssignHandler.AddReference(game, source, field, info, extraData)
	if info.comment then
		field:ParseComment(game, info.comment)
	end
end

return FieldHandler