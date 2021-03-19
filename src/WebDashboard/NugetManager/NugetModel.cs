﻿using System.Collections.Generic;
using System.Linq;
using Melville.MVVM.FileSystem;

namespace WebDashboard.NugetManager
{
    public class NugetModel
    {
        public string Version { get; }
        public IList<ProjectFile> Files { get; }
        public ProjectGraph Graph { get; }

        public NugetModel(string version, IList<ProjectFile> files)
        {
            Version = version;
            Files = files;
            Graph = new ProjectGraph();
            PopulateGraph();
        }

        private void PopulateGraph()
        {
            foreach (var vertex in Files)
            {
                Graph.AddVertex(vertex);
            }

            foreach (var source in Files)
            {
                foreach (var target in source.DependsOn)
                {
                    Graph.AddEdge(new ProjectEdge(source, target));
                }
            }
        }

    }
}