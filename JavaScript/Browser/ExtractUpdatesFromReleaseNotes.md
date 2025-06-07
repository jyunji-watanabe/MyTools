# Extracting Updates from Release Notes as JavaScript Object
## How to use this code
1. Open the release notes page in your browser (This script is tested for OutSystems 11 > Platform Server on 2025/06/07).
2. Open the browser's developer console (usually F12 or right-click and select "Inspect").
3. Copy and paste the code snippet below into the console.
4. Press Enter to run the code.
5. The result variable will contain the structured data extracted from the release notes.

## Code Snippet
``` javascript
// Get the target tags from the document
let targetTags = document.querySelector("[data-block='WebBlock.InjectHMTL']").childNodes[0].children;

const result = [];
let currentVersion = null;

Array.from(targetTags).forEach(node => {
  if (node.tagName === "H2") {
    if (currentVersion) {
      delete currentVersion._currentCategory;
      result.push(currentVersion);
    }
    currentVersion = { VersionNo: node.innerText, categories: {} };
  } else if (node.tagName === "H3" && currentVersion) {
    currentVersion._currentCategory = node.innerText;
    // If the _currentCategory starts with "New", use just "New" as the category name
    if (currentVersion._currentCategory.startsWith("New")) {
      currentVersion._currentCategory = "New";
    }
    // ['New', 'Bug Fixing', 'Known issues', 'Breaking change', 'Known Issues', 'Limitation', 'Known issue', 'Breaking Change', 'Breaking changes', 'Breaking Changes'] -> Correct fluctuation ['New', 'Bug Fixing', 'Known Issues', 'Breaking Change', 'Limitation']
    if (currentVersion._currentCategory === "Bug Fixing") {
      currentVersion._currentCategory = "Bug Fixing";
    } else if (currentVersion._currentCategory === "Known issues" || currentVersion._currentCategory === "Known issue") {
      currentVersion._currentCategory = "Known Issues";
    } else if (currentVersion._currentCategory === "Breaking change" || currentVersion._currentCategory === "Breaking changes" || currentVersion._currentCategory === "Breaking Change") {
      currentVersion._currentCategory = "Breaking Changes";
    } else if (currentVersion._currentCategory === "Limitation") {
      currentVersion._currentCategory = "Limitation";
    }

    currentVersion.categories[currentVersion._currentCategory] = [];
  } else if (node.tagName === "UL" && currentVersion && currentVersion._currentCategory) {
    const items = Array.from(node.querySelectorAll("li")).map(li => li.innerText.trim().replace(/"/g, '""')); // replace " with ""
    currentVersion.categories[currentVersion._currentCategory].push(...items);
  }
});
if (currentVersion) {
  delete currentVersion._currentCategory;
  result.push(currentVersion);
}

console.log(result);
```

# Converting the result to CSV
## How to use this code
1. After running the first code snippet, copy and paste the code snippet below into the console.
2. Press Enter to run the code.
3. The CSV string will be logged to the console.
## Code Snippet

``` javascript
const csvRows = [];
// Add header row
csvRows.push("VersionNo,Category,Update");
// Iterate over each version
result.forEach(version => {
  const versionNo = version.VersionNo;
  // Iterate over each category in the version
  Object.keys(version.categories).forEach(category => {
    const updates = version.categories[category];
    updates.forEach(update => {
      csvRows.push(`"${versionNo}","${category}","${update}"`);
    });
  });
});
// Join all rows into a single CSV string
const csvString = csvRows.join("\n");
console.log(csvString);
```
