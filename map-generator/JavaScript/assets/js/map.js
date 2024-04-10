var canvas = document.getElementById("map");
var ctx = canvas.getContext("2d");
var currentColor = 0;
var map = new Array(50);
for (var i = 0; i < 50; i++) {
    map[i] = new Array(50);
    for (var j = 0; j < 50; j++) {
        map[i][j] = 0;
    }
}
var color = [
    "#FFFFFF", // Space
    "#B97A57", // Ruin
    "#22B14C", // Shadow
    "#99D9EA", // Asteroid
    "#A349A4", // Resource
    "#FF7F27", // Construction
    "#880015", // Wormhole
    "#ED1C24", // Home
];

const placeType = {
    Space: 0,
    Ruin: 1,
    Shadow: 2,
    Asteroid: 3,
    Resource: 4,
    Construction: 5,
    Wormhole: 6,
    Home: 7,
};

function draw() {
    ctx.clearRect(0, 0, 500, 500);
    for (var i = 0; i < 50; i++) {
        for (var j = 0; j < 50; j++) {
            ctx.fillStyle = color[map[i][j]];
            ctx.fillRect(i * 10, j * 10, 10, 10);
        }
    }
}

canvas.onmousedown = function (e) {
    var x = Math.floor(e.offsetX / 10);
    var y = Math.floor(e.offsetY / 10);
    map[x][y] = currentColor;
    draw();
}

document.getElementById("space").onclick = function () {
    currentColor = 0;
    document.getElementById("current").innerHTML = "当前：Space";
}
document.getElementById("ruin").onclick = function () {
    currentColor = 1;
    document.getElementById("current").innerHTML = "当前：Ruin";
}
document.getElementById("shadow").onclick = function () {
    currentColor = 2;
    document.getElementById("current").innerHTML = "当前：Shadow";
}
document.getElementById("asteroid").onclick = function () {
    currentColor = 3;
    document.getElementById("current").innerHTML = "当前：Asteroid";
}
document.getElementById("resource").onclick = function () {
    currentColor = 4;
    document.getElementById("current").innerHTML = "当前：Resource";
}
document.getElementById("construction").onclick = function () {
    currentColor = 5;
    document.getElementById("current").innerHTML = "当前：Construction";
}
document.getElementById("wormhole").onclick = function () {
    currentColor = 6;
    document.getElementById("current").innerHTML = "当前：Wormhole";
}
document.getElementById("home").onclick = function () {
    currentColor = 7;
    document.getElementById("current").innerHTML = "当前：Home";
}

function saveAsTxt() {
    var str = "";
    for (var i = 0; i < 50; i++) {
        for (var j = 0; j < 50; j++) {
            str += map[j][i];
            str += " ";
        }
        str += "\n";
    }
    var a = document.createElement("a");
    a.style.display = "none";
    a.href = "data:text/plain;charset=utf-8," + str;
    a.download = "map.txt";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

function saveAsCs() {
    var str = "public static uint[,] Map = new uint[50, 50] {\n";
    for (var i = 0; i < 50; i++) {
        str += "    {";
        for (var j = 0; j < 50; j++) {
            str += map[j][i];
            if (j != 49) {
                str += ", ";
            }
        }
        str += "}";
        if (i != 49) {
            str += ",";
        }
        str += "\n";
    }
    str += "};";
    var a = document.createElement("a");
    a.style.display = "none";
    a.href = "data:text/plain;charset=utf-8," + str;
    a.download = "map.cs";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

function saveAsPng() {
    var a = document.createElement("a");
    a.style.display = "none";
    a.href = canvas.toDataURL("image/png");
    a.download = "map.png";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

function isEmptyNearby(x, y, radius) {
    for (var i = (x - radius >= 0 ? x - radius : 0); i <= (x + radius <= 49 ? x + radius : 49); i++) {
        for (var j = (y - radius >= 0 ? y - radius : 0); j <= (y + radius <= 49 ? y + radius : 49); j++) {
            if (map[i][j] != 0) {
                return false;
            }
        }
    }
    return true;
}

function haveSthNearby(x, y, radius, type) {
    var ret = 0;
    for (var i = (x - radius >= 0 ? x - radius : 0); i <= (x + radius <= 49 ? x + radius : 49); i++) {
        for (var j = (y - radius >= 0 ? y - radius : 0); j <= (y + radius <= 49 ? y + radius : 49); j++) {
            if (map[i][j] == type) {
                ret++;
            }
        }
    }
    return ret;
}

function haveSthCross(x, y, radius, type) {
    var ret = 0;
    for (var i = (x - radius >= 0 ? x - radius : 0); i <= (x + radius <= 49 ? x + radius : 49); i++) {
        if (map[i][y] == type) {
            ret++;
        }
    }
    for (var j = (y - radius >= 0 ? y - radius : 0); j <= (y + radius <= 49 ? y + radius : 49); j++) {
        if (map[x][j] == type) {
            ret++;
        }
    }
    return ret;
}

function generateBorderRuin() {
    for (var i = 0; i < 50; i++) {
        map[i][0] = placeType.Ruin;
        map[i][49] = placeType.Ruin;
        map[0][i] = placeType.Ruin;
        map[49][i] = placeType.Ruin;
    }
}

function generateHome() {
    map[3][46] = placeType.Home;
    map[46][3] = placeType.Home;
}

function generateAsteroid(width = 2) {
    for (var i = 1; i < 49; i++) {
        for (var j = 24; j > 24 - width; j--) {
            map[i][j] = placeType.Asteroid;
            map[49 - i][49 - j] = placeType.Asteroid;
        }
    }
    for (var i = 1; i < 23; i++) {
        if (Math.random() < 0.5 && i != 9 && i != 10 && i != 11 && i != 12) {
            map[i][24 - width] = placeType.Asteroid;
            map[i][24 + width] = placeType.Space;
            map[49 - i][25 + width] = placeType.Asteroid;
            map[49 - i][25 - width] = placeType.Space;
        }
    }
}

function generateResource(num = 7) {
    for (var i = 0; i < num; i++) {
        var x = Math.floor(Math.random() * 48) + 1;
        var y = Math.floor(Math.random() * 23) + 1;
        if (isEmptyNearby(x, y, 2)) {
            map[x][y] = placeType.Resource;
            map[49 - x][49 - y] = placeType.Resource;
        }
        else {
            i--;
        }
    }
}

function generateConstruction(num = 5) {
    for (var i = 0; i < num; i++) {
        var x = Math.floor(Math.random() * 48) + 1;
        var y = Math.floor(Math.random() * 23) + 1;
        if (isEmptyNearby(x, y, 1)) {
            map[x][y] = placeType.Construction;
            map[49 - x][49 - y] = placeType.Construction;
        }
        else {
            i--;
        }
    }
}

function generateShadow(prob = 0.015, crossBonus = 23) {
    for (var i = 0; i < 50; i++) {
        for (var j = 0; j < 50; j++) {
            if (map[i][j] == 0 && Math.random() < prob * (haveSthCross(i, j, 1, placeType.Shadow) * crossBonus + 1)) {
                map[i][j] = placeType.Shadow;
                map[49 - i][49 - j] = placeType.Shadow;
            }
        }
    }
}

function generateRuin(prob = 0.01, crossBonus = 40) {
    for (var i = 2; i < 48; i++) {
        for (var j = 2; j < 48; j++) {
            if ((map[i][j] == 0 || map[i][j] == 2) &&
                !haveSthNearby(i, j, 1, placeType.Asteroid) &&
                !haveSthNearby(i, j, 1, placeType.Home) &&
                Math.random() < prob * (haveSthCross(i, j, 1, placeType.Ruin) * (haveSthCross(i, j, 1, placeType.Ruin) > 1 ? 0 : crossBonus) + 1)) {
                map[i][j] = 1;
                map[49 - i][49 - j] = 1;
            }
        }
    }
}

function generateWormhole() {
    for (var i = 1; i < 49; i++) {
        if (map[10][i] == 3) {
            map[10][i] = placeType.Wormhole;
            map[39][49 - i] = placeType.Wormhole;
        }
        if (map[11][i] == 3) {
            map[11][i] = placeType.Wormhole;
            map[38][49 - i] = placeType.Wormhole;
        }
        if (map[24][i] == 3) {
            map[24][i] = placeType.Wormhole;
            map[25][49 - i] = placeType.Wormhole;
        }
    }
}

function clearCanvas() {
    for (var i = 0; i < 50; i++) {
        map[i] = new Array(50);
        for (var j = 0; j < 50; j++) {
            map[i][j] = 0;
        }
    }
    // generateBorderRuin();
    // generateHome();
    draw();
}

function random() {
    for (var i = 0; i < 50; i++) {
        map[i] = new Array(50);
        for (var j = 0; j < 50; j++) {
            map[i][j] = 0;
        }
    }
    var asteroidWidth = parseInt(document.getElementById("asteroid-width").value);
    var resourceNum = parseInt(document.getElementById("resource-num").value);
    var constructionNum = parseInt(document.getElementById("construction-num").value);
    var shadowProb = parseFloat(document.getElementById("shadow-prob").value);
    var shadowCrossBonus = parseInt(document.getElementById("shadow-cross-bonus").value);
    var ruinProb = parseFloat(document.getElementById("ruin-prob").value);
    var ruinCrossBonus = parseInt(document.getElementById("ruin-cross-bonus").value);
    if (isNaN(asteroidWidth) || asteroidWidth < 1 || asteroidWidth > 4) {
        asteroidWidth = 2;
        alert("Asteroid 宽度非法，设置为默认值 2");
        document.getElementById("asteroid-width").value = 2;
    }
    if (isNaN(resourceNum) || resourceNum < 1 || resourceNum > 10) {
        resourceNum = 7;
        alert("Resource 数量非法，设置为默认值 7");
        document.getElementById("resource-num").value = 7;
    }
    if (isNaN(constructionNum) || constructionNum < 1 || constructionNum > 10) {
        constructionNum = 5;
        alert("Construction 数量非法，设置为默认值 5");
        document.getElementById("construction-num").value = 5;
    }
    if (isNaN(shadowProb) || shadowProb < 0 || shadowProb > 0.1) {
        shadowProb = 0.015;
        alert("Shadow 生成概率非法，设置为默认值 0.015");
        document.getElementById("shadow-prob").value = 0.015;
    }
    if (isNaN(shadowCrossBonus) || shadowCrossBonus < 1 || shadowCrossBonus > 50) {
        shadowCrossBonus = 23;
        alert("Shadow 蔓延加成非法，设置为默认值 23");
        document.getElementById("shadow-cross-bonus").value = 23;
    }
    if (isNaN(ruinProb) || ruinProb < 0 || ruinProb > 0.1) {
        ruinProb = 0.01;
        alert("Ruin 生成概率非法，设置为默认值 0.01");
        document.getElementById("ruin-prob").value = 0.01;
    }
    if (isNaN(ruinCrossBonus) || ruinCrossBonus < 1 || ruinCrossBonus > 50) {
        ruinCrossBonus = 40;
        alert("Ruin 蔓延加成非法，设置为默认值 40");
        document.getElementById("ruin-cross-bonus").value = 40;
    }
    generateBorderRuin();
    generateHome();
    generateAsteroid(asteroidWidth);
    generateResource(resourceNum);
    generateConstruction(constructionNum);
    generateShadow(shadowProb, shadowCrossBonus);
    generateRuin(ruinProb, ruinCrossBonus);
    generateWormhole();
    draw();
}

clearCanvas();
