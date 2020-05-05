using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.IO;

namespace CellularAutomata.GameOfLife
{
    /// <summary>
    /// Uses the Game of Life Azure function hosted in the cloud.
    /// </summary>
    public static class CellularAutomataWorldCloudCompute
    {

        /// <summary>
        /// Uses the Cellular Automata Game of Life Azure Function.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public static async Task NextGenerationUsingCloudAsync(this CellularAutomataWorld world)
        {
            string json = JsonConvert.SerializeObject(world);
            StringContent sc = new StringContent(json);
            HttpClient hc = new HttpClient();
            string url = "https://cellularautomatagol.azurewebsites.net/api/SimulateNextGeneration";
            HttpResponseMessage hrm = await hc.PostAsync(url, sc);
            Stream s = await hrm.Content.ReadAsStreamAsync();
            StreamReader sr = new StreamReader(s);
            JsonSerializer js = new JsonSerializer();
            CellularAutomataWorld caw = (CellularAutomataWorld)js.Deserialize(sr, typeof(CellularAutomataWorld));
            world.Cells = caw.Cells;
        }
    }
}