# README #

Here you can find some useful information to deploy the program
### What is this documentation for? ###

* Creating a Squirrel Release
* Share the Releases
* Manually updating with squirrel

### Creating a Squirrel Release ###

For creating a new release a few steps are necessary:

* first you have to make sure that your solution contains now errors & warnings
* then build your application by using the Release option
* then we have to create a nuget package:
    * open the **Nuget Package Explorer**
    * select **File -> New**
    * edit the package metadata by adding the name, version and other and make sure there are no dependencies listed.
    * To the *Package contents* section add a *Lib folder*
    * inside this folder add a .net v4.5 folder
    * then right click on this folder and select **Add existing File ...**
    * now you select all files inside the /bin/Release folder except: *.pdb, *.vshost.exe* 
    * then go to ** File -> Save Metadata As...** and save the *.nuspec* file
* after this step insert the following command inside the cmd: **nuget.exe pack James.<version>.nuspec** this step will create you the nuget package. This step will generate you a *James.<version>.nupkg*
* The final step is to create the Squirrel release.
    * open the **Package Manager Console** inside the VS.
    * Run the following command: **Squirrel --releasify .\James.<version>.nupkg**. This will create you the **Release folder** including the following files: setup.exe, the delta files, the nuget packages as well as the RELEASES file.
* After you created the new release version the whole *Release* folder should be synced with the webspace's *Release* folder.


### Share the Releases ###

For sharing the releases a normal nginx is used. To run independently from the host docker is used.
For the Webserver the offical nginx image is used: [Docker Hub](https://hub.docker.com/_/nginx/).

On your host you should have a folder with this file tree:
```
Releases
   - changelog.txt
   - auto generated Squirrel files.
```

To start the container simple run the following command in the parent directory of from the *Release*:
```
docker run --name JamesReleasesNginx -v ~/James/Releases:/usr/share/nginx/html/Releases:ro -d -p 80:80 nginx
```

This command will run a new container with the name *JamesReleasesNginx* and the mounted directory *Releases* as a deamon container. In addition the the hosts 80 port will be forwarded inside the container.

Now it's possible to fetch the data from the server by using the http protocol. For example:
```
curl <host>/Releases/changelog.txt
```
will fetch you the changelog.

### Manually updating with squirrel ###
With Squirrel it's possible to update manually by using the following command:
```
Update.exe --update <host>\Releases
```
