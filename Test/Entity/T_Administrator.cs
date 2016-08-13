//*******************************
// Create By Rocher Kong 
// Date 2016-04-17 15:07
//*******************************

using System;

namespace Test.Entity
{
    /// <summary>
    ///T_Administrator
    /// </summary>	
    [Serializable]
    public class T_Administrator
    {
        /// <summary>
        /// Id
        /// </summary>		
        public long Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>		
        public string Name { get; set; }

        /// <summary>
        /// Age
        /// </summary>		
        public int Age { get; set; }

        /// <summary>
        /// LoginName
        /// </summary>		
        public string LoginName { get; set; }

        /// <summary>
        /// Password
        /// </summary>		
        public string Password { get; set; }


    }
}