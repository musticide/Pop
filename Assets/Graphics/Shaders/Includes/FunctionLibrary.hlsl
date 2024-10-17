half Raymarch(half3 ro, half3 rd, int maxSteps, half surfaceDist, half maxDist){
    half dO = 0;
    half dS;
    for(int i = 0; i < maxSteps; i++){
        half3 p = ro + dO * rd;
        // #ifdef SDF_FUNCTION 
            // dS = SDF_FUNCTION(p);
        //     return 0.5h;
        // #endif
        dS = SDFFunction(p);
        dO += dS;
        if (dS< surfaceDist || dO > maxDist) break;
    }
    return dO;
}

half3 GetNormal(half3 p){
    half2 o = half2(.01, 0);
    half3 n = SDFFunction(p) - (half3(
                SDFFunction(p - o.xyy),
                SDFFunction(p - o.yxy),
                SDFFunction(p - o.yyx)
    ));
    return normalize(n);
}

