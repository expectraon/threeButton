using System.Collections;
namespace Global
{
    public static class GLOBAL_VAR
    {
        public class Layer
        {
            public float oneLayerPercent;
            public float twoLayerPercent;
        }

        public class Phase
        {
            public Layer[] layersInfo;
            public int[] layerCount;
            public float speed;
            public Phase()
            {
                layersInfo = new Layer[4];
                for(int i=0;i<layersInfo.Length;i++)
                {
                    layersInfo[i] = new Layer();
                }
                layerCount = new int[4];
            }
        }

        public static int spawnCount = 40;   //한 페이즈에 몇개의 타겟이 나올지
        public static Phase[] phaseInfo = new Phase[100];   //페이즈별 정보
        public static void LoadData()
        {
            for(int i=0;i<phaseInfo.Length;i++)
            {
                phaseInfo[i] = new Phase();
            }
            TxtParser.LoadData("data");
        }
    }
}