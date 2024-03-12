import Uppy from '@uppy/core';
import Dashboard from '@uppy/dashboard';
import Tus from '@uppy/tus';

import '@uppy/core/dist/style.min.css';
import '@uppy/dashboard/dist/style.min.css';

const uppyDashboard = new Uppy()
  .use(Dashboard, { inline: true, target: '#app', showProgressDetails: true, proudlyDisplayPoweredByUppy: false})
  .use(Tus, { endpoint: 'http://localhost:5001/files/', limit: 6 });


window.uppy = uppyDashboard;

uppyDashboard.on("complete", (result) => {
  if (result.failed.length === 0) {
    console.log("Upload successful");
  } else {
    console.warn("Upload failed");
  }
  console.log("successful files:", result.successful);
  console.log("failed files:", result.failed);
});