function downloadLatestBuildArtifact() {
	fetch("https://api.github.com/repos/PuddleduckProductions/MysticForestParkRanger/actions/runs").then((res) => {
		return res.json();
	}).then((json) => {
		var workflowRuns = json.workflow_runs;
		for (let r of workflowRuns) {
			if (r.name === "Unity Build and Test") {
				fetch(r.artifacts_url).then(res => res.json()).then((json) => {
					var artifacts = json.artifacts;
					for (let a of artifacts) {
						if (a.name === "Build") {
							var url = a.url.replace("https://api.github.com/repos/", "https://github.com/");
							url = url.replace("/actions/", `/actions/runs/${r.id}/`);
							location.replace(url);
							var w = window.open(r.html_url + "#artifacts");
							break;
						}
					}
				});
				break;
			}
		}
	});
}