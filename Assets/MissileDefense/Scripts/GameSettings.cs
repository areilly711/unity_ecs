/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Unity.Entities;

namespace MissileDefense
{
    /// <summary>
    /// Difficulty settings for the game
    /// </summary>
    [GenerateAuthoringComponent]
    public class GameSettings : IComponentData
    {
        /// <summary>
        /// how quickly the missiles spawn, in seconds
        /// </summary>
        public float spawnRate;

        /// <summary>
        /// the number of spawns when the spawn rate is ready
        /// </summary>
        public int spawns;

        /// <summary>
        /// minimum missile speed
        /// </summary>
        public float speedMin;

        /// <summary>
        /// maximum missile speed
        /// </summary>
        public float speedMax;

        /// <summary>
        /// minimum x pos to spawn
        /// </summary>
        public float posMin;

        /// <summary>
        /// maximum x pos to spawn
        /// </summary>
        public float posMax;

        /// <summary>
        /// starting missile y pos
        /// </summary>
        public float spawnYPos;

        /// <summary>
        /// destory the missile when its y pos drops below this value
        /// </summary>
        public float worldEndThreshold;        
    }
}
