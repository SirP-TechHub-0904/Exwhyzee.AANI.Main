 

function adjustImageHeight(dataValue) {
    const image = document.querySelector('.image' + dataValue + ' img');
    const t1 = document.querySelector('.title' + dataValue);
    const b1 = document.querySelector('.button' + dataValue);
    const textc = document.querySelector('.text' + dataValue);
    const heightyy = image.height;
    console.log(heightyy);
    console.log("img");
    const xb1 = b1.offsetHeight;
    console.log(xb1);
    console.log("btn");
    const xt1 = t1.offsetHeight;
    console.log(xt1);
    console.log("tn");
    const newh1 = heightyy - (xb1 + xt1) - 16;
    console.log(newh1);
    console.log("new");
    textc.style.height = newh1 + 'px';
}
 






