const gulp = require('gulp');
var debug = require('gulp-debug');
const concat = require('gulp-concat');
const uglify = require('gulp-uglify');
var runSequence = require('gulp4-run-sequence');
const msbuild = require('gulp-msbuild');
const foreach = require('gulp-foreach');
const glob = require('glob'); // Additional plugin for dynamic folder structure
var gulpConfig = require("./gulp-config.js")();
module.exports.config = gulpConfig;
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

var publishProjects = function (location, dest) {
    dest = dest || gulpConfig.webRoot;
    var targets = ["Clean", "Build"];

    console.log("publish to " + dest + " folder");

    return gulp.src([location + "/**/*.csproj"])
        .pipe(foreach(function (stream, file) {
            return stream
                .pipe(debug({ title: "Building project:" }))
                .pipe(msbuild({
                    targets: targets,
                    configuration: gulpConfig.buildConfiguration,
                    logCommand: false,
                    verbosity: "minimal",
                    stdout: true,
                    errorOnFail: true,
                    maxcpucount: 0,
                    toolsVersion: gulpConfig.MSBuildToolsVersion,
                    properties: {
                        DeployOnBuild: "true",
                        DeployDefaultTarget: "WebPublish",
                        WebPublishMethod: "FileSystem",
                        DeleteExistingFiles: "false",
                        publishUrl: dest,
                        _FindDependencies: "false"
                    }
                }));
        }));
};
gulp.task("Build-Solution", function () {
    var targets = ["Clean", "Build"];

    return gulp.src("C:/TouneyHub/TourneyHub/TourneyHub.sln")
        .pipe(msbuild({
            targets: targets,
            configuration: gulpConfig.buildConfiguration,
            logCommand: false,
            verbosity: "minimal",
            stdout: true,
            errorOnFail: true,
            maxcpucount: 0,
            toolsVersion: gulpConfig.MSBuildToolsVersion
        }));
});

gulp.task("Publish-All-Projects", function (callback) {
    return runSequence(
        "Build-Solution",
        "Publish-Feature-Projects",
        "Publish-Project-Projects", callback);
});

//gulp.task("Publish-Foundation-Projects", function () {
//    return publishProjects("./src/Foundation");
//});

gulp.task("Publish-Feature-Projects", function () {
    return publishProjects("C:/TouneyHub/TourneyHub/src/Feature");
});



gulp.task("Publish-Project-Projects", function () {
    return publishProjects("C:/TouneyHub/TourneyHub/src/project");
});
gulp.task('default', gulp.series('scripts'));
