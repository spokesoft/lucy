const fs = require('fs');
const path = require('path');

const dirsToDelete = ['node_modules', '.react-router', 'build'];

dirsToDelete.forEach(dir => {
  const dirPath = path.join(process.cwd(), dir);
  if (fs.existsSync(dirPath)) {
    console.log(`Removing ${dir}...`);
    fs.rmSync(dirPath, { recursive: true, force: true });
  }
});

console.log('Cleanup complete.');