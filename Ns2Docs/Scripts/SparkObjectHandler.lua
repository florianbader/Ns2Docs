local Declaration = Ns2Docs.Spark.Declaration

local SparkObjectHandler = {}

function SparkObjectHandler.Init(game, source, object, info, extraData)
	local filePosition = source:GetFilePosition(info.offset)
	
	object.DeclaredIn = source
	object.Line = filePosition.Line
	object.Column = filePosition.Column
	object.Offset = info.offset
	
	if info.offsetEnd then
		local filePositionEnd = source:GetFilePosition(info.offsetEnd)
		object.LineEnd = filePositionEnd.Line
		object.ColumnEnd = filePositionEnd.Column
		object.OffsetEnd = info.offsetEnd
	end
	
	
	if extraData.Library then
		object.Library = extraData.Library
	end
	
	if info.comment then
		object:ParseComment(game, info.comment)
	end
end

return SparkObjectHandler