float3 modulo(float3 divident, float3 divisor) {
	float3 positiveDivident = divident % divisor + divisor;
	return positiveDivident % divisor;
}
//float3 rand3dTo3d(float3 val, float offset)
//{
//	float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
//	float2 UV = frac(sin(mul(float2(val.x, val.y), m)) * 46839.32);
//	return float3(sin(UV.y * +offset) * 0.5 + 0.5, sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
//}
float rand3dTo1d(float3 value, float3 dotDir, float1 angleOffset){
	//make value smaller to avoid artefacts
	float3 smallValue = sin(value);
	//get scalar value from 3d vector
	float random = dot(smallValue, dotDir);
	//make value more random by making it bigger and then taking the factional part
	random = frac(sin(random) * 143758.5453);
	return random;
}

float3 rand3dTo3d(float3 value, float1 angleOffset){
	return float3(
		rand3dTo1d(value, float3(12.989, 78.233, 37.719), angleOffset),
		rand3dTo1d(value, float3(39.346, 11.135, 83.155), angleOffset),
		rand3dTo1d(value, float3(73.156, 52.235, 09.151), angleOffset)
	);
}

float3 voronoiNoise(float3 value, float3 period, float1 angleOffset){
	float3 baseCell = floor(value);

	float minDistToCell = 10;
	float3 toClosestCell;
	float3 closestCell;
	for(int x1=-1; x1<=1; x1++){
		for(int y1=-1; y1<=1; y1++){
			for(int z1=-1; z1<=1; z1++){
				float3 cell = baseCell + float3(x1, y1, z1);
				float3 tiledCell = modulo(cell, period);
				float3 cellPosition = cell + rand3dTo3d(tiledCell, angleOffset);
				float3 toCell = cellPosition - value;
				float distToCell = length(toCell);
				if(distToCell < minDistToCell){
					minDistToCell = distToCell;
					closestCell = cell;
					toClosestCell = toCell;
				}
			}
		}
	}

	float minEdgeDistance = 10;
	for(int x2=-1; x2<=1; x2++){
		for(int y2=-1; y2<=1; y2++){
			for(int z2=-1; z2<=1; z2++){
				float3 cell = baseCell + float3(x2, y2, z2);
				float3 tiledCell = modulo(cell, period);
				float3 cellPosition = cell + rand3dTo3d(tiledCell, angleOffset);
				float3 toCell = cellPosition - value;

				float3 diffToClosestCell = abs(closestCell - cell);
				bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y + diffToClosestCell.z < 0.1;
				if(!isClosestCell){
					float3 toCenter = (toClosestCell + toCell) * 0.5;
					float3 cellDifference = normalize(toCell - toClosestCell);
					float edgeDistance = dot(toCenter, cellDifference);
					minEdgeDistance = min(minEdgeDistance, edgeDistance);
				}
			}
		}
	}

	float random = rand3dTo1d(closestCell, float3(12.9898, 78.233, 37.719), angleOffset);
	return float3(minDistToCell, random, minEdgeDistance);
}
float3 CalculateVoronoi_float(float2 UV, float1 Height, float1 CellDensity, float3 Period, float1 AngleOffset, out float1 Voronoi, out float1 Cells, out float1 CellBorder){

	float3 value = float3(UV, Height) * CellDensity;
	float3 noise = voronoiNoise(value, Period, AngleOffset);


	Voronoi = noise.x;
	Cells = noise.y;
	CellBorder = noise.z;
	return Voronoi;
}