const gulp = require('gulp');
const concat = require('gulp-concat');
const uglify = require('gulp-uglify');
const glob = require('glob'); // Additional plugin for dynamic folder structure

// Define a function to get all feature folders
function getFeatureFolders() {
    return glob.sync('ScriptsFrontend/*/'); // Matches any subdirectory under Scripts
}

// Define a Gulp task to process the JavaScript files
gulp.task('scripts', function () {
    const featureFolders = getFeatureFolders();

    // Create an array of file globs for each feature's JavaScript files
    const jsFiles = featureFolders.map((folder) => folder + '*.js');

    return gulp.src(jsFiles)
        .pipe(concat('bundle.js')) // Concatenate all JavaScript files into one bundle
        .pipe(uglify()) // Minify the bundle (optional)
        .pipe(gulp.dest('dist'));
});

gulp.task('default', gulp.series('scripts'));
