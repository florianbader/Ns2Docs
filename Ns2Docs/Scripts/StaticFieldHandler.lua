local AssignHandler = require "AssignHandler"
local PotentialStaticField = Ns2Docs.Spark.PotentialStaticField
local Table = Ns2Docs.Spark.Table
local StaticField = Ns2Docs.Spark.StaticField
local StaticFieldHandler = {}

function StaticFieldHandler.Capture(offset, tableName, member, assignment, offsetEnd)
	return 
	{
		__name='StaticField', 
		tableName=tableName,
		member=member,
		assignment=assignment,
		line=line,
		offset=offset,
		offsetEnd=offsetEnd,
		Create = StaticFieldHandler.Create
	}
end



function StaticFieldHandler.Create(game, source, info, extraData)
	local member = info.member
	local fieldNameEnd = member:find("[%.%[]")
	local staticFieldName = member:sub(1, fieldNameEnd)
	
	
	
	local potentialStaticField = PotentialStaticField(info.tableName, staticFieldName, info.assignment)
	potentialStaticField.OnReaped = function(sparkTable)
		local staticField = StaticField(sparkTable, potentialStaticField:get_FieldName())
		staticField:set_Assignment(potentialStaticField.assignment)
		AssignHandler.SetUpVariable(game, source, staticField, info, extraData)
		sparkTable:get_StaticFields():Add(staticField)
		
		AssignHandler.AddReference(game, source, staticField, info, extraData)
	end
	game:get_PotentialStaticFields():Add(potentialStaticField)
	
	return potentialStaticField
end



return StaticFieldHandler