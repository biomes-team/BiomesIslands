using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BiomesIslands.GenSteps
{
    [StaticConstructorOnStartup]
    public static class WorldMaterials
    {

        public static readonly Material Atoll;
        public static readonly Material TropicalIsland;
        static WorldMaterials()
        {
            Atoll = MaterialPool.MatFrom("World/MapGraphics/Atoll", ShaderDatabase.WorldOverlayTransparentLit, 3505);
            TropicalIsland = MaterialPool.MatFrom("World/MapGraphics/TropicalIsland", ShaderDatabase.WorldOverlayTransparentLit, 3505);
        }
    }

    [DefOf]
    public static class BiomesExtraIslandsWorldMapGraphicClass
    {
        public static BiomeDef BiomesIslands_Atoll;
        public static BiomeDef BiomesIslands_TropicalIsland;


        static BiomesExtraIslandsWorldMapGraphicClass()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BiomesExtraIslandsWorldMapGraphicClass));
        }
    }

    public class BiomesAtollMaker : WorldLayer
    {
        private static readonly IntVec2 TexturesInAtlas = new IntVec2(2, 2);

        public override IEnumerable Regenerate()
        {
            foreach (object obj in base.Regenerate())
            {
                yield return obj;
            }
            Rand.PushState();
            Rand.Seed = Find.World.info.Seed;
            WorldGrid worldGrid = Find.WorldGrid;
            for (int i = 0; i < worldGrid.TilesCount; i++)
            {
                if (worldGrid.tiles[i].biome != BiomesExtraIslandsWorldMapGraphicClass.BiomesIslands_Atoll)
                    continue;
                Material material = WorldMaterials.Atoll;
                LayerSubMesh subMesh = base.GetSubMesh(material);
                Vector3 vector = worldGrid.GetTileCenter(i);
                WorldRendererUtility.PrintQuadTangentialToPlanet(vector, vector, worldGrid.averageTileSize, 0.01f, subMesh, false, false, false);
                WorldRendererUtility.PrintTextureAtlasUVs(Rand.Range(0, TexturesInAtlas.x), Rand.Range(0, TexturesInAtlas.z), TexturesInAtlas.x, TexturesInAtlas.z, subMesh);
            }
            Rand.PopState();
            base.FinalizeMesh(MeshParts.All);
            yield break;
        }
    }

    public class BiomesTropicalIslandMaker : WorldLayer
    {
        private static readonly IntVec2 TexturesInAtlas = new IntVec2(2, 2);

        public override IEnumerable Regenerate()
        {
            foreach (object obj in base.Regenerate())
            {
                yield return obj;
            }
            Rand.PushState();
            Rand.Seed = Find.World.info.Seed;
            WorldGrid worldGrid = Find.WorldGrid;
            for (int i = 0; i < worldGrid.TilesCount; i++)
            {
                if (worldGrid.tiles[i].biome != BiomesExtraIslandsWorldMapGraphicClass.BiomesIslands_TropicalIsland)
                    continue;
                Material material = WorldMaterials.TropicalIsland;
                LayerSubMesh subMesh = base.GetSubMesh(material);
                Vector3 vector = worldGrid.GetTileCenter(i);
                WorldRendererUtility.PrintQuadTangentialToPlanet(vector, vector, worldGrid.averageTileSize, 0.01f, subMesh, false, false, false);
                WorldRendererUtility.PrintTextureAtlasUVs(Rand.Range(0, TexturesInAtlas.x), Rand.Range(0, TexturesInAtlas.z), TexturesInAtlas.x, TexturesInAtlas.z, subMesh);
            }
            Rand.PopState();
            base.FinalizeMesh(MeshParts.All);
            yield break;
        }
    }
}
