class EmotionsExperiment {

    constructor(callbacks) {
        this.videoFrame = {};
        this.listenerSet = false;
        this.currentConfig = -1;
        this.stages =
            [
                {
                    name: "FaceEmotionsStream",
                    type: "video",
                    file: "assets/stimulus/movie/FaceEmotionsStream.webm",
                    frameRate: 29.97,
                    annotations: {
                        1: {
                            "caption": "FACE-01"
                        },
                        154: {
                            "caption": "FACE-02"
                        },
                        308: {
                            "caption": "FACE-03"
                        },
                        462: {
                            "caption": "FACE-04"
                        },
                        613: {
                            "caption": "FACE-05"
                        },
                        767: {
                            "caption": "FACE-06"
                        },
                        918: {
                            "caption": "FACE-07"
                        },
                        1072: {
                            "caption": "FACE-08"
                        },
                        1226: {
                            "caption": "FACE-09"
                        },
                        1380: {
                            "caption": "FACE-10"
                        },
                        1534: {
                            "caption": "FACE-11"
                        },
                        1685: {
                            "caption": "FACE-12"
                        },
                        1840: {
                            "caption": "FACE-13"
                        },
                        1995: {
                            "caption": "FACE-14"
                        },
                    },
                    annotateEveryFrame: false,
                    callbacks: {
                        "progress": function (stageId, data) {
                            callbacks["progress"](stageId, data);
                        },
                        "done": function (stageId, data) {
                            callbacks["done"](stageId, data);
                        },
                        "annotate": function (stageId, data) {
                            callbacks["annotate"](stageId, data);
                        }
                    }
                },
                {
                    name: "WSEFEP - 1s",
                    type: "video",
                    file: "assets/stimulus/movie/_wsefep_1s_30fps.webm",
                    frameRate: 30.0,
                    annotations: {
                        1: { 'caption': 'AA_0000.jpg' },
                        30: { 'caption': 'JG_2291.jpg' },
                        60: { 'caption': 'MS_0695.jpg' },
                        60: { 'caption': 'RB_0586.jpg' },
                        90: { 'caption': 'KS_3640.jpg' },
                        120: { 'caption': 'MR2_0896.jpg' },
                        150: { 'caption': 'PO_0951.jpg' },
                        180: { 'caption': 'HW_1612.jpg' },
                        210: { 'caption': 'KO_0251.jpg' },
                        240: { 'caption': 'MK_0173.jpg' },
                        270: { 'caption': 'PA_0967.jpg' },
                        300: { 'caption': 'RA_1786.jpg' },
                        330: { 'caption': 'KL_1182.jpg' },
                        360: { 'caption': 'MB_1210.jpg' },
                        390: { 'caption': 'AG_1314.jpg' },
                        420: { 'caption': 'PB_0314.jpg' },
                        450: { 'caption': 'MG_1317.jpg' },
                        480: { 'caption': 'KM_0620.jpg' },
                        510: { 'caption': 'OG_6484.jpg' },
                        540: { 'caption': 'KA_1616.jpg' },
                        570: { 'caption': 'SS_0302.jpg' },
                        600: { 'caption': 'MR_2450.jpg' },
                        630: { 'caption': 'PS_0100.jpg' },
                        660: { 'caption': 'MK1_0461.jpg' },
                        690: { 'caption': 'SO_0071.jpg' },
                        720: { 'caption': 'JS_2296.jpg' },
                        750: { 'caption': 'AD_9681.jpg' },
                        780: { 'caption': 'MJ_0152.jpg' },
                        810: { 'caption': 'DC_1317.jpg' },
                        840: { 'caption': 'KP_0760.jpg' },
                        870: { 'caption': 'MR1_1802.jpg' },
                        900: { 'caption': 'AD_8268.jpg' },
                        930: { 'caption': 'MS_0627.jpg' },
                        960: { 'caption': 'HW_0452.jpg' },
                        990: { 'caption': 'PO_0673.jpg' },
                        1020: { 'caption': 'JS_0744.jpg' },
                        1050: { 'caption': 'KL_0697.jpg' },
                        1080: { 'caption': 'MK_0496.jpg' },
                        1110: { 'caption': 'MR2_1290.jpg' },
                        1140: { 'caption': 'KM_1583.jpg' },
                        1170: { 'caption': 'MK1_0411.jpg' },
                        1200: { 'caption': 'RA_2267.jpg' },
                        1230: { 'caption': 'SO_0053.jpg' },
                        1260: { 'caption': 'KA_0535.jpg' },
                        1290: { 'caption': 'PA_1701.jpg' },
                        1320: { 'caption': 'DC_0616.jpg' },
                        1350: { 'caption': 'PS_0719.jpg' },
                        1380: { 'caption': 'MJ_0370.jpg' },
                        1410: { 'caption': 'MR1_1199.jpg' },
                        1440: { 'caption': 'MB_2133.jpg' },
                        1470: { 'caption': 'SS_0084.jpg' },
                        1500: { 'caption': 'KO_0484.jpg' },
                        1530: { 'caption': 'KP_0351.jpg' },
                        1560: { 'caption': 'PB_0442.jpg' },
                        1590: { 'caption': 'AG_0666.jpg' },
                        1620: { 'caption': 'MG_0928.jpg' },
                        1650: { 'caption': 'OG_6390.jpg' },
                        1680: { 'caption': 'JG_0134.jpg' },
                        1710: { 'caption': 'KS_0993.jpg' },
                        1740: { 'caption': 'MR_1669.jpg' },
                        1770: { 'caption': 'RB_0167.jpg' },
                        1800: { 'caption': 'MS_0226.jpg' },
                        1830: { 'caption': 'MJ_0271.jpg' },
                        1860: { 'caption': 'PO_1030.jpg' },
                        1890: { 'caption': 'OG_7620.jpg' },
                        1920: { 'caption': 'MK1_0746.jpg' },
                        1950: { 'caption': 'AD_8432.jpg' },
                        1980: { 'caption': 'PB_0269.jpg' },
                        2010: { 'caption': 'AG_0424.jpg' },
                        2040: { 'caption': 'JG_1465.jpg' },
                        2070: { 'caption': 'KL_0900.jpg' },
                        2100: { 'caption': 'MK_0364.jpg' },
                        2130: { 'caption': 'MR1_1519.jpg' },
                        2160: { 'caption': 'RB_0392.jpg' },
                        2190: { 'caption': 'DC_0952.jpg' },
                        2220: { 'caption': 'KO_1082.jpg' },
                        2250: { 'caption': 'JS_1601.jpg' },
                        2280: { 'caption': 'KA_1134.jpg' },
                        2310: { 'caption': 'MB_1031.jpg' },
                        2340: { 'caption': 'KM_1980.jpg' },
                        2370: { 'caption': 'SO_1515.jpg' },
                        2400: { 'caption': 'PA_0394.jpg' },
                        2430: { 'caption': 'RA_3483.jpg' },
                        2460: { 'caption': 'MG_1280.jpg' },
                        2490: { 'caption': 'PS_0157.jpg' },
                        2520: { 'caption': 'MR_0959.jpg' },
                        2550: { 'caption': 'KS_0624.jpg' },
                        2580: { 'caption': 'KP_1148.jpg' },
                        2610: { 'caption': 'SS_1188.jpg' },
                        2640: { 'caption': 'HW_2219.jpg' },
                        2670: { 'caption': 'MR2_1829.jpg' },
                        2700: { 'caption': 'MR_0055.jpg' },
                        2730: { 'caption': 'KS_4222.jpg' },
                        2760: { 'caption': 'MK_0040.jpg' },
                        2790: { 'caption': 'SS_0018.jpg' },
                        2820: { 'caption': 'AG_0086.jpg' },
                        2850: { 'caption': 'MJ_0066.jpg' },
                        2880: { 'caption': 'MR2_0063.jpg' },
                        2910: { 'caption': 'MS_0291.jpg' },
                        2940: { 'caption': 'KL_0092.jpg' },
                        2970: { 'caption': 'KO_0277.jpg' },
                        3000: { 'caption': 'DC_0139.jpg' },
                        3030: { 'caption': 'JS_0281.jpg' },
                        3060: { 'caption': 'PS_0236.jpg' },
                        3090: { 'caption': 'RB_0329.jpg' },
                        3120: { 'caption': 'KA_0043.jpg' },
                        3150: { 'caption': 'PA_0112.jpg' },
                        3180: { 'caption': 'PB_0499.jpg' },
                        3210: { 'caption': 'RA_0317.jpg' },
                        3240: { 'caption': 'AD_7950.jpg' },
                        3270: { 'caption': 'MG_0330.jpg' },
                        3300: { 'caption': 'HW_0068.jpg' },
                        3330: { 'caption': 'JG_0024.jpg' },
                        3360: { 'caption': 'KM_0137.jpg' },
                        3390: { 'caption': 'PO_0124.jpg' },
                        3420: { 'caption': 'OG_6189.jpg' },
                        3450: { 'caption': 'KP_0051.jpg' },
                        3480: { 'caption': 'MK1_0087.jpg' },
                        3510: { 'caption': 'MR1_0132.jpg' },
                        3540: { 'caption': 'SO_0028.jpg' },
                        3570: { 'caption': 'MB_0048.jpg' },
                        3600: { 'caption': 'HW_0006.jpg' },
                        3630: { 'caption': 'MJ_0346.jpg' },
                        3660: { 'caption': 'PB_0001.jpg' },
                        3690: { 'caption': 'RB_0006.jpg' },
                        3720: { 'caption': 'PS_0216.jpg' },
                        3750: { 'caption': 'AD_7885.jpg' },
                        3780: { 'caption': 'KA_0003.jpg' },
                        3810: { 'caption': 'MK_0001.jpg' },
                        3840: { 'caption': 'KM_0017.jpg' },
                        3870: { 'caption': 'PO_0015.jpg' },
                        3900: { 'caption': 'KS_2161.jpg' },
                        3930: { 'caption': 'MK1_0007.jpg' },
                        3960: { 'caption': 'AG_0011.jpg' },
                        3990: { 'caption': 'KL_0024.jpg' },
                        4020: { 'caption': 'MR1_0006.jpg' },
                        4050: { 'caption': 'MS_0004.jpg' },
                        4080: { 'caption': 'MR_0013.jpg' },
                        4110: { 'caption': 'MB_0026.jpg' },
                        4140: { 'caption': 'DC_0014.jpg' },
                        4170: { 'caption': 'MR2_0014.jpg' },
                        4200: { 'caption': 'RA_0057.jpg' },
                        4230: { 'caption': 'MG_0345.jpg' },
                        4260: { 'caption': 'KO_0031.jpg' },
                        4290: { 'caption': 'JG_1226.jpg' },
                        4320: { 'caption': 'PA_0006.jpg' },
                        4350: { 'caption': 'SS_0151.jpg' },
                        4380: { 'caption': 'JS_0008.jpg' },
                        4410: { 'caption': 'OG_6108.jpg' },
                        4440: { 'caption': 'SO_2188.jpg' },
                        4470: { 'caption': 'KP_0082.jpg' },
                        4500: { 'caption': 'KP_0991.jpg' },
                        4530: { 'caption': 'RA_1215.jpg' },
                        4560: { 'caption': 'OG_6566.jpg' },
                        4590: { 'caption': 'KM_1295.jpg' },
                        4620: { 'caption': 'PB_0144.jpg' },
                        4650: { 'caption': 'MR1_0821.jpg' },
                        4680: { 'caption': 'SS_0539.jpg' },
                        4710: { 'caption': 'KO_0665.jpg' },
                        4740: { 'caption': 'SO_0893.jpg' },
                        4770: { 'caption': 'DC_1399.jpg' },
                        4800: { 'caption': 'AD_8595.jpg' },
                        4830: { 'caption': 'MK_0306.jpg' },
                        4860: { 'caption': 'RB_0458.jpg' },
                        4890: { 'caption': 'PA_1348.jpg' },
                        4920: { 'caption': 'MJ_0484.jpg' },
                        4950: { 'caption': 'KA_2396.jpg' },
                        4980: { 'caption': 'JG_1632.jpg' },
                        5010: { 'caption': 'JS_2987.jpg' },
                        5040: { 'caption': 'HW_2478.jpg' },
                        5070: { 'caption': 'PS_0746.jpg' },
                        5100: { 'caption': 'KL_1438.jpg' },
                        5130: { 'caption': 'MG_0754.jpg' },
                        5160: { 'caption': 'MR2_2086.jpg' },
                        5190: { 'caption': 'MS_0104.jpg' },
                        5220: { 'caption': 'MR_2767.jpg' },
                        5250: { 'caption': 'PO_0843.jpg' },
                        5280: { 'caption': 'AG_1460.jpg' },
                        5310: { 'caption': 'KS_2052.jpg' },
                        5340: { 'caption': 'MB_2362.jpg' },
                        5370: { 'caption': 'MK1_1982.jpg' },
                        5400: { 'caption': 'DC_0272.jpg' },
                        5430: { 'caption': 'KP_0225.jpg' },
                        5460: { 'caption': 'MK1_1427.jpg' },
                        5490: { 'caption': 'PA_0840.jpg' },
                        5520: { 'caption': 'JS_0449.jpg' },
                        5550: { 'caption': 'PB_1383.jpg' },
                        5580: { 'caption': 'RA_2800.jpg' },
                        5610: { 'caption': 'AG_0283.jpg' },
                        5640: { 'caption': 'RB_0255.jpg' },
                        5670: { 'caption': 'JG_1329.jpg' },
                        5700: { 'caption': 'MB_0400.jpg' },
                        5730: { 'caption': 'MR_0619.jpg' },
                        5760: { 'caption': 'PO_0553.jpg' },
                        5790: { 'caption': 'AD_8397.jpg' },
                        5820: { 'caption': 'SS_0032.jpg' },
                        5850: { 'caption': 'KO_0624.jpg' },
                        5880: { 'caption': 'OG_7702.jpg' },
                        5910: { 'caption': 'KL_0324.jpg' },
                        5940: { 'caption': 'MG_1069.jpg' },
                        5970: { 'caption': 'MJ_0332.jpg' },
                        6000: { 'caption': 'MR2_0580.jpg' },
                        6030: { 'caption': 'PS_0282.jpg' },
                        6060: { 'caption': 'HW_2057.jpg' },
                        6090: { 'caption': 'MK_0255.jpg' },
                        6120: { 'caption': 'MR1_1418.jpg' },
                        6150: { 'caption': 'KA_0884.jpg' },
                        6180: { 'caption': 'KS_0252.jpg' },
                        6210: { 'caption': 'KM_1797.jpg' },
                        6240: { 'caption': 'MS_0431.jpg' },
                        6270: { 'caption': 'SO_0223.jpg' },
                        6300: { 'caption': 'AA_0000.jpg' },
                    },
                    annotateEveryFrame: false,
                    callbacks: {
                        "progress": function (stageId, data) {
                            callbacks["progress"](stageId, data);
                        },
                        "done": function (stageId, data) {
                            callbacks["done"](stageId, data);
                        },
                        "annotate": function (stageId, data) {
                            callbacks["annotate"](stageId, data);
                        }
                    }
                },
                {
                    name: "WSEFEP - 2s",
                    type: "video",
                    file: "assets/stimulus/movie/_wsefep_2s_30fps.webm",
                    frameRate: 30.0,
                    annotations: {
                        1: { 'caption': 'AA_0000.jpg' },
                        60: { 'caption': 'JG_2291.jpg' },
                        120: { 'caption': 'MS_0695.jpg' },
                        120: { 'caption': 'RB_0586.jpg' },
                        180: { 'caption': 'KS_3640.jpg' },
                        240: { 'caption': 'MR2_0896.jpg' },
                        300: { 'caption': 'PO_0951.jpg' },
                        360: { 'caption': 'HW_1612.jpg' },
                        420: { 'caption': 'KO_0251.jpg' },
                        480: { 'caption': 'MK_0173.jpg' },
                        540: { 'caption': 'PA_0967.jpg' },
                        600: { 'caption': 'RA_1786.jpg' },
                        660: { 'caption': 'KL_1182.jpg' },
                        720: { 'caption': 'MB_1210.jpg' },
                        780: { 'caption': 'AG_1314.jpg' },
                        840: { 'caption': 'PB_0314.jpg' },
                        900: { 'caption': 'MG_1317.jpg' },
                        960: { 'caption': 'KM_0620.jpg' },
                        1020: { 'caption': 'OG_6484.jpg' },
                        1080: { 'caption': 'KA_1616.jpg' },
                        1140: { 'caption': 'SS_0302.jpg' },
                        1200: { 'caption': 'MR_2450.jpg' },
                        1260: { 'caption': 'PS_0100.jpg' },
                        1320: { 'caption': 'MK1_0461.jpg' },
                        1380: { 'caption': 'SO_0071.jpg' },
                        1440: { 'caption': 'JS_2296.jpg' },
                        1500: { 'caption': 'AD_9681.jpg' },
                        1560: { 'caption': 'MJ_0152.jpg' },
                        1620: { 'caption': 'DC_1317.jpg' },
                        1680: { 'caption': 'KP_0760.jpg' },
                        1740: { 'caption': 'MR1_1802.jpg' },
                        1800: { 'caption': 'AD_8268.jpg' },
                        1860: { 'caption': 'MS_0627.jpg' },
                        1920: { 'caption': 'HW_0452.jpg' },
                        1980: { 'caption': 'PO_0673.jpg' },
                        2040: { 'caption': 'JS_0744.jpg' },
                        2100: { 'caption': 'KL_0697.jpg' },
                        2160: { 'caption': 'MK_0496.jpg' },
                        2220: { 'caption': 'MR2_1290.jpg' },
                        2280: { 'caption': 'KM_1583.jpg' },
                        2340: { 'caption': 'MK1_0411.jpg' },
                        2400: { 'caption': 'RA_2267.jpg' },
                        2460: { 'caption': 'SO_0053.jpg' },
                        2520: { 'caption': 'KA_0535.jpg' },
                        2580: { 'caption': 'PA_1701.jpg' },
                        2640: { 'caption': 'DC_0616.jpg' },
                        2700: { 'caption': 'PS_0719.jpg' },
                        2760: { 'caption': 'MJ_0370.jpg' },
                        2820: { 'caption': 'MR1_1199.jpg' },
                        2880: { 'caption': 'MB_2133.jpg' },
                        2940: { 'caption': 'SS_0084.jpg' },
                        3000: { 'caption': 'KO_0484.jpg' },
                        3060: { 'caption': 'KP_0351.jpg' },
                        3120: { 'caption': 'PB_0442.jpg' },
                        3180: { 'caption': 'AG_0666.jpg' },
                        3240: { 'caption': 'MG_0928.jpg' },
                        3300: { 'caption': 'OG_6390.jpg' },
                        3360: { 'caption': 'JG_0134.jpg' },
                        3420: { 'caption': 'KS_0993.jpg' },
                        3480: { 'caption': 'MR_1669.jpg' },
                        3540: { 'caption': 'RB_0167.jpg' },
                        3600: { 'caption': 'MS_0226.jpg' },
                        3660: { 'caption': 'MJ_0271.jpg' },
                        3720: { 'caption': 'PO_1030.jpg' },
                        3780: { 'caption': 'OG_7620.jpg' },
                        3840: { 'caption': 'MK1_0746.jpg' },
                        3900: { 'caption': 'AD_8432.jpg' },
                        3960: { 'caption': 'PB_0269.jpg' },
                        4020: { 'caption': 'AG_0424.jpg' },
                        4080: { 'caption': 'JG_1465.jpg' },
                        4140: { 'caption': 'KL_0900.jpg' },
                        4200: { 'caption': 'MK_0364.jpg' },
                        4260: { 'caption': 'MR1_1519.jpg' },
                        4320: { 'caption': 'RB_0392.jpg' },
                        4380: { 'caption': 'DC_0952.jpg' },
                        4440: { 'caption': 'KO_1082.jpg' },
                        4500: { 'caption': 'JS_1601.jpg' },
                        4560: { 'caption': 'KA_1134.jpg' },
                        4620: { 'caption': 'MB_1031.jpg' },
                        4680: { 'caption': 'KM_1980.jpg' },
                        4740: { 'caption': 'SO_1515.jpg' },
                        4800: { 'caption': 'PA_0394.jpg' },
                        4860: { 'caption': 'RA_3483.jpg' },
                        4920: { 'caption': 'MG_1280.jpg' },
                        4980: { 'caption': 'PS_0157.jpg' },
                        5040: { 'caption': 'MR_0959.jpg' },
                        5100: { 'caption': 'KS_0624.jpg' },
                        5160: { 'caption': 'KP_1148.jpg' },
                        5220: { 'caption': 'SS_1188.jpg' },
                        5280: { 'caption': 'HW_2219.jpg' },
                        5340: { 'caption': 'MR2_1829.jpg' },
                        5400: { 'caption': 'MR_0055.jpg' },
                        5460: { 'caption': 'KS_4222.jpg' },
                        5520: { 'caption': 'MK_0040.jpg' },
                        5580: { 'caption': 'SS_0018.jpg' },
                        5640: { 'caption': 'AG_0086.jpg' },
                        5700: { 'caption': 'MJ_0066.jpg' },
                        5760: { 'caption': 'MR2_0063.jpg' },
                        5820: { 'caption': 'MS_0291.jpg' },
                        5880: { 'caption': 'KL_0092.jpg' },
                        5940: { 'caption': 'KO_0277.jpg' },
                        6000: { 'caption': 'DC_0139.jpg' },
                        6060: { 'caption': 'JS_0281.jpg' },
                        6120: { 'caption': 'PS_0236.jpg' },
                        6180: { 'caption': 'RB_0329.jpg' },
                        6240: { 'caption': 'KA_0043.jpg' },
                        6300: { 'caption': 'PA_0112.jpg' },
                        6360: { 'caption': 'PB_0499.jpg' },
                        6420: { 'caption': 'RA_0317.jpg' },
                        6480: { 'caption': 'AD_7950.jpg' },
                        6540: { 'caption': 'MG_0330.jpg' },
                        6600: { 'caption': 'HW_0068.jpg' },
                        6660: { 'caption': 'JG_0024.jpg' },
                        6720: { 'caption': 'KM_0137.jpg' },
                        6780: { 'caption': 'PO_0124.jpg' },
                        6840: { 'caption': 'OG_6189.jpg' },
                        6900: { 'caption': 'KP_0051.jpg' },
                        6960: { 'caption': 'MK1_0087.jpg' },
                        7020: { 'caption': 'MR1_0132.jpg' },
                        7080: { 'caption': 'SO_0028.jpg' },
                        7140: { 'caption': 'MB_0048.jpg' },
                        7200: { 'caption': 'HW_0006.jpg' },
                        7260: { 'caption': 'MJ_0346.jpg' },
                        7320: { 'caption': 'PB_0001.jpg' },
                        7380: { 'caption': 'RB_0006.jpg' },
                        7440: { 'caption': 'PS_0216.jpg' },
                        7500: { 'caption': 'AD_7885.jpg' },
                        7560: { 'caption': 'KA_0003.jpg' },
                        7620: { 'caption': 'MK_0001.jpg' },
                        7680: { 'caption': 'KM_0017.jpg' },
                        7740: { 'caption': 'PO_0015.jpg' },
                        7800: { 'caption': 'KS_2161.jpg' },
                        7860: { 'caption': 'MK1_0007.jpg' },
                        7920: { 'caption': 'AG_0011.jpg' },
                        7980: { 'caption': 'KL_0024.jpg' },
                        8040: { 'caption': 'MR1_0006.jpg' },
                        8100: { 'caption': 'MS_0004.jpg' },
                        8160: { 'caption': 'MR_0013.jpg' },
                        8220: { 'caption': 'MB_0026.jpg' },
                        8280: { 'caption': 'DC_0014.jpg' },
                        8340: { 'caption': 'MR2_0014.jpg' },
                        8400: { 'caption': 'RA_0057.jpg' },
                        8460: { 'caption': 'MG_0345.jpg' },
                        8520: { 'caption': 'KO_0031.jpg' },
                        8580: { 'caption': 'JG_1226.jpg' },
                        8640: { 'caption': 'PA_0006.jpg' },
                        8700: { 'caption': 'SS_0151.jpg' },
                        8760: { 'caption': 'JS_0008.jpg' },
                        8820: { 'caption': 'OG_6108.jpg' },
                        8880: { 'caption': 'SO_2188.jpg' },
                        8940: { 'caption': 'KP_0082.jpg' },
                        9000: { 'caption': 'KP_0991.jpg' },
                        9060: { 'caption': 'RA_1215.jpg' },
                        9120: { 'caption': 'OG_6566.jpg' },
                        9180: { 'caption': 'KM_1295.jpg' },
                        9240: { 'caption': 'PB_0144.jpg' },
                        9300: { 'caption': 'MR1_0821.jpg' },
                        9360: { 'caption': 'SS_0539.jpg' },
                        9420: { 'caption': 'KO_0665.jpg' },
                        9480: { 'caption': 'SO_0893.jpg' },
                        9540: { 'caption': 'DC_1399.jpg' },
                        9600: { 'caption': 'AD_8595.jpg' },
                        9660: { 'caption': 'MK_0306.jpg' },
                        9720: { 'caption': 'RB_0458.jpg' },
                        9780: { 'caption': 'PA_1348.jpg' },
                        9840: { 'caption': 'MJ_0484.jpg' },
                        9900: { 'caption': 'KA_2396.jpg' },
                        9960: { 'caption': 'JG_1632.jpg' },
                        10020: { 'caption': 'JS_2987.jpg' },
                        10080: { 'caption': 'HW_2478.jpg' },
                        10140: { 'caption': 'PS_0746.jpg' },
                        10200: { 'caption': 'KL_1438.jpg' },
                        10260: { 'caption': 'MG_0754.jpg' },
                        10320: { 'caption': 'MR2_2086.jpg' },
                        10380: { 'caption': 'MS_0104.jpg' },
                        10440: { 'caption': 'MR_2767.jpg' },
                        10500: { 'caption': 'PO_0843.jpg' },
                        10560: { 'caption': 'AG_1460.jpg' },
                        10620: { 'caption': 'KS_2052.jpg' },
                        10680: { 'caption': 'MB_2362.jpg' },
                        10740: { 'caption': 'MK1_1982.jpg' },
                        10800: { 'caption': 'DC_0272.jpg' },
                        10860: { 'caption': 'KP_0225.jpg' },
                        10920: { 'caption': 'MK1_1427.jpg' },
                        10980: { 'caption': 'PA_0840.jpg' },
                        11040: { 'caption': 'JS_0449.jpg' },
                        11100: { 'caption': 'PB_1383.jpg' },
                        11160: { 'caption': 'RA_2800.jpg' },
                        11220: { 'caption': 'AG_0283.jpg' },
                        11280: { 'caption': 'RB_0255.jpg' },
                        11340: { 'caption': 'JG_1329.jpg' },
                        11400: { 'caption': 'MB_0400.jpg' },
                        11460: { 'caption': 'MR_0619.jpg' },
                        11520: { 'caption': 'PO_0553.jpg' },
                        11580: { 'caption': 'AD_8397.jpg' },
                        11640: { 'caption': 'SS_0032.jpg' },
                        11700: { 'caption': 'KO_0624.jpg' },
                        11760: { 'caption': 'OG_7702.jpg' },
                        11820: { 'caption': 'KL_0324.jpg' },
                        11880: { 'caption': 'MG_1069.jpg' },
                        11940: { 'caption': 'MJ_0332.jpg' },
                        12000: { 'caption': 'MR2_0580.jpg' },
                        12060: { 'caption': 'PS_0282.jpg' },
                        12120: { 'caption': 'HW_2057.jpg' },
                        12180: { 'caption': 'MK_0255.jpg' },
                        12240: { 'caption': 'MR1_1418.jpg' },
                        12300: { 'caption': 'KA_0884.jpg' },
                        12360: { 'caption': 'KS_0252.jpg' },
                        12420: { 'caption': 'KM_1797.jpg' },
                        12480: { 'caption': 'MS_0431.jpg' },
                        12540: { 'caption': 'SO_0223.jpg' },
                        12600: { 'caption': 'AA_0000.jpg' },
                    },
                    annotateEveryFrame: false,
                    callbacks: {
                        "progress": function (stageId, data) {
                            callbacks["progress"](stageId, data);
                        },
                        "done": function (stageId, data) {
                            callbacks["done"](stageId, data);
                        },
                        "annotate": function (stageId, data) {
                            callbacks["annotate"](stageId, data);
                        }
                    }
                },
                {
                    name: "WSEFEP - 3s",
                    type: "video",
                    file: "assets/stimulus/movie/_wsefep_3s_30fps.webm",
                    frameRate: 30.0,
                    annotations: {
                        1: { 'caption': 'AA_0000.jpg' },
                        90: { 'caption': 'JG_2291.jpg' },
                        180: { 'caption': 'MS_0695.jpg' },
                        180: { 'caption': 'RB_0586.jpg' },
                        270: { 'caption': 'KS_3640.jpg' },
                        360: { 'caption': 'MR2_0896.jpg' },
                        450: { 'caption': 'PO_0951.jpg' },
                        540: { 'caption': 'HW_1612.jpg' },
                        630: { 'caption': 'KO_0251.jpg' },
                        720: { 'caption': 'MK_0173.jpg' },
                        810: { 'caption': 'PA_0967.jpg' },
                        900: { 'caption': 'RA_1786.jpg' },
                        990: { 'caption': 'KL_1182.jpg' },
                        1080: { 'caption': 'MB_1210.jpg' },
                        1170: { 'caption': 'AG_1314.jpg' },
                        1260: { 'caption': 'PB_0314.jpg' },
                        1350: { 'caption': 'MG_1317.jpg' },
                        1440: { 'caption': 'KM_0620.jpg' },
                        1530: { 'caption': 'OG_6484.jpg' },
                        1620: { 'caption': 'KA_1616.jpg' },
                        1710: { 'caption': 'SS_0302.jpg' },
                        1800: { 'caption': 'MR_2450.jpg' },
                        1890: { 'caption': 'PS_0100.jpg' },
                        1980: { 'caption': 'MK1_0461.jpg' },
                        2070: { 'caption': 'SO_0071.jpg' },
                        2160: { 'caption': 'JS_2296.jpg' },
                        2250: { 'caption': 'AD_9681.jpg' },
                        2340: { 'caption': 'MJ_0152.jpg' },
                        2430: { 'caption': 'DC_1317.jpg' },
                        2520: { 'caption': 'KP_0760.jpg' },
                        2610: { 'caption': 'MR1_1802.jpg' },
                        2700: { 'caption': 'AD_8268.jpg' },
                        2790: { 'caption': 'MS_0627.jpg' },
                        2880: { 'caption': 'HW_0452.jpg' },
                        2970: { 'caption': 'PO_0673.jpg' },
                        3060: { 'caption': 'JS_0744.jpg' },
                        3150: { 'caption': 'KL_0697.jpg' },
                        3240: { 'caption': 'MK_0496.jpg' },
                        3330: { 'caption': 'MR2_1290.jpg' },
                        3420: { 'caption': 'KM_1583.jpg' },
                        3510: { 'caption': 'MK1_0411.jpg' },
                        3600: { 'caption': 'RA_2267.jpg' },
                        3690: { 'caption': 'SO_0053.jpg' },
                        3780: { 'caption': 'KA_0535.jpg' },
                        3870: { 'caption': 'PA_1701.jpg' },
                        3960: { 'caption': 'DC_0616.jpg' },
                        4050: { 'caption': 'PS_0719.jpg' },
                        4140: { 'caption': 'MJ_0370.jpg' },
                        4230: { 'caption': 'MR1_1199.jpg' },
                        4320: { 'caption': 'MB_2133.jpg' },
                        4410: { 'caption': 'SS_0084.jpg' },
                        4500: { 'caption': 'KO_0484.jpg' },
                        4590: { 'caption': 'KP_0351.jpg' },
                        4680: { 'caption': 'PB_0442.jpg' },
                        4770: { 'caption': 'AG_0666.jpg' },
                        4860: { 'caption': 'MG_0928.jpg' },
                        4950: { 'caption': 'OG_6390.jpg' },
                        5040: { 'caption': 'JG_0134.jpg' },
                        5130: { 'caption': 'KS_0993.jpg' },
                        5220: { 'caption': 'MR_1669.jpg' },
                        5310: { 'caption': 'RB_0167.jpg' },
                        5400: { 'caption': 'MS_0226.jpg' },
                        5490: { 'caption': 'MJ_0271.jpg' },
                        5580: { 'caption': 'PO_1030.jpg' },
                        5670: { 'caption': 'OG_7620.jpg' },
                        5760: { 'caption': 'MK1_0746.jpg' },
                        5850: { 'caption': 'AD_8432.jpg' },
                        5940: { 'caption': 'PB_0269.jpg' },
                        6030: { 'caption': 'AG_0424.jpg' },
                        6120: { 'caption': 'JG_1465.jpg' },
                        6210: { 'caption': 'KL_0900.jpg' },
                        6300: { 'caption': 'MK_0364.jpg' },
                        6390: { 'caption': 'MR1_1519.jpg' },
                        6480: { 'caption': 'RB_0392.jpg' },
                        6570: { 'caption': 'DC_0952.jpg' },
                        6660: { 'caption': 'KO_1082.jpg' },
                        6750: { 'caption': 'JS_1601.jpg' },
                        6840: { 'caption': 'KA_1134.jpg' },
                        6930: { 'caption': 'MB_1031.jpg' },
                        7020: { 'caption': 'KM_1980.jpg' },
                        7110: { 'caption': 'SO_1515.jpg' },
                        7200: { 'caption': 'PA_0394.jpg' },
                        7290: { 'caption': 'RA_3483.jpg' },
                        7380: { 'caption': 'MG_1280.jpg' },
                        7470: { 'caption': 'PS_0157.jpg' },
                        7560: { 'caption': 'MR_0959.jpg' },
                        7650: { 'caption': 'KS_0624.jpg' },
                        7740: { 'caption': 'KP_1148.jpg' },
                        7830: { 'caption': 'SS_1188.jpg' },
                        7920: { 'caption': 'HW_2219.jpg' },
                        8010: { 'caption': 'MR2_1829.jpg' },
                        8100: { 'caption': 'MR_0055.jpg' },
                        8190: { 'caption': 'KS_4222.jpg' },
                        8280: { 'caption': 'MK_0040.jpg' },
                        8370: { 'caption': 'SS_0018.jpg' },
                        8460: { 'caption': 'AG_0086.jpg' },
                        8550: { 'caption': 'MJ_0066.jpg' },
                        8640: { 'caption': 'MR2_0063.jpg' },
                        8730: { 'caption': 'MS_0291.jpg' },
                        8820: { 'caption': 'KL_0092.jpg' },
                        8910: { 'caption': 'KO_0277.jpg' },
                        9000: { 'caption': 'DC_0139.jpg' },
                        9090: { 'caption': 'JS_0281.jpg' },
                        9180: { 'caption': 'PS_0236.jpg' },
                        9270: { 'caption': 'RB_0329.jpg' },
                        9360: { 'caption': 'KA_0043.jpg' },
                        9450: { 'caption': 'PA_0112.jpg' },
                        9540: { 'caption': 'PB_0499.jpg' },
                        9630: { 'caption': 'RA_0317.jpg' },
                        9720: { 'caption': 'AD_7950.jpg' },
                        9810: { 'caption': 'MG_0330.jpg' },
                        9900: { 'caption': 'HW_0068.jpg' },
                        9990: { 'caption': 'JG_0024.jpg' },
                        10080: { 'caption': 'KM_0137.jpg' },
                        10170: { 'caption': 'PO_0124.jpg' },
                        10260: { 'caption': 'OG_6189.jpg' },
                        10350: { 'caption': 'KP_0051.jpg' },
                        10440: { 'caption': 'MK1_0087.jpg' },
                        10530: { 'caption': 'MR1_0132.jpg' },
                        10620: { 'caption': 'SO_0028.jpg' },
                        10710: { 'caption': 'MB_0048.jpg' },
                        10800: { 'caption': 'HW_0006.jpg' },
                        10890: { 'caption': 'MJ_0346.jpg' },
                        10980: { 'caption': 'PB_0001.jpg' },
                        11070: { 'caption': 'RB_0006.jpg' },
                        11160: { 'caption': 'PS_0216.jpg' },
                        11250: { 'caption': 'AD_7885.jpg' },
                        11340: { 'caption': 'KA_0003.jpg' },
                        11430: { 'caption': 'MK_0001.jpg' },
                        11520: { 'caption': 'KM_0017.jpg' },
                        11610: { 'caption': 'PO_0015.jpg' },
                        11700: { 'caption': 'KS_2161.jpg' },
                        11790: { 'caption': 'MK1_0007.jpg' },
                        11880: { 'caption': 'AG_0011.jpg' },
                        11970: { 'caption': 'KL_0024.jpg' },
                        12060: { 'caption': 'MR1_0006.jpg' },
                        12150: { 'caption': 'MS_0004.jpg' },
                        12240: { 'caption': 'MR_0013.jpg' },
                        12330: { 'caption': 'MB_0026.jpg' },
                        12420: { 'caption': 'DC_0014.jpg' },
                        12510: { 'caption': 'MR2_0014.jpg' },
                        12600: { 'caption': 'RA_0057.jpg' },
                        12690: { 'caption': 'MG_0345.jpg' },
                        12780: { 'caption': 'KO_0031.jpg' },
                        12870: { 'caption': 'JG_1226.jpg' },
                        12960: { 'caption': 'PA_0006.jpg' },
                        13050: { 'caption': 'SS_0151.jpg' },
                        13140: { 'caption': 'JS_0008.jpg' },
                        13230: { 'caption': 'OG_6108.jpg' },
                        13320: { 'caption': 'SO_2188.jpg' },
                        13410: { 'caption': 'KP_0082.jpg' },
                        13500: { 'caption': 'KP_0991.jpg' },
                        13590: { 'caption': 'RA_1215.jpg' },
                        13680: { 'caption': 'OG_6566.jpg' },
                        13770: { 'caption': 'KM_1295.jpg' },
                        13860: { 'caption': 'PB_0144.jpg' },
                        13950: { 'caption': 'MR1_0821.jpg' },
                        14040: { 'caption': 'SS_0539.jpg' },
                        14130: { 'caption': 'KO_0665.jpg' },
                        14220: { 'caption': 'SO_0893.jpg' },
                        14310: { 'caption': 'DC_1399.jpg' },
                        14400: { 'caption': 'AD_8595.jpg' },
                        14490: { 'caption': 'MK_0306.jpg' },
                        14580: { 'caption': 'RB_0458.jpg' },
                        14670: { 'caption': 'PA_1348.jpg' },
                        14760: { 'caption': 'MJ_0484.jpg' },
                        14850: { 'caption': 'KA_2396.jpg' },
                        14940: { 'caption': 'JG_1632.jpg' },
                        15030: { 'caption': 'JS_2987.jpg' },
                        15120: { 'caption': 'HW_2478.jpg' },
                        15210: { 'caption': 'PS_0746.jpg' },
                        15300: { 'caption': 'KL_1438.jpg' },
                        15390: { 'caption': 'MG_0754.jpg' },
                        15480: { 'caption': 'MR2_2086.jpg' },
                        15570: { 'caption': 'MS_0104.jpg' },
                        15660: { 'caption': 'MR_2767.jpg' },
                        15750: { 'caption': 'PO_0843.jpg' },
                        15840: { 'caption': 'AG_1460.jpg' },
                        15930: { 'caption': 'KS_2052.jpg' },
                        16020: { 'caption': 'MB_2362.jpg' },
                        16110: { 'caption': 'MK1_1982.jpg' },
                        16200: { 'caption': 'DC_0272.jpg' },
                        16290: { 'caption': 'KP_0225.jpg' },
                        16380: { 'caption': 'MK1_1427.jpg' },
                        16470: { 'caption': 'PA_0840.jpg' },
                        16560: { 'caption': 'JS_0449.jpg' },
                        16650: { 'caption': 'PB_1383.jpg' },
                        16740: { 'caption': 'RA_2800.jpg' },
                        16830: { 'caption': 'AG_0283.jpg' },
                        16920: { 'caption': 'RB_0255.jpg' },
                        17010: { 'caption': 'JG_1329.jpg' },
                        17100: { 'caption': 'MB_0400.jpg' },
                        17190: { 'caption': 'MR_0619.jpg' },
                        17280: { 'caption': 'PO_0553.jpg' },
                        17370: { 'caption': 'AD_8397.jpg' },
                        17460: { 'caption': 'SS_0032.jpg' },
                        17550: { 'caption': 'KO_0624.jpg' },
                        17640: { 'caption': 'OG_7702.jpg' },
                        17730: { 'caption': 'KL_0324.jpg' },
                        17820: { 'caption': 'MG_1069.jpg' },
                        17910: { 'caption': 'MJ_0332.jpg' },
                        18000: { 'caption': 'MR2_0580.jpg' },
                        18090: { 'caption': 'PS_0282.jpg' },
                        18180: { 'caption': 'HW_2057.jpg' },
                        18270: { 'caption': 'MK_0255.jpg' },
                        18360: { 'caption': 'MR1_1418.jpg' },
                        18450: { 'caption': 'KA_0884.jpg' },
                        18540: { 'caption': 'KS_0252.jpg' },
                        18630: { 'caption': 'KM_1797.jpg' },
                        18720: { 'caption': 'MS_0431.jpg' },
                        18810: { 'caption': 'SO_0223.jpg' },
                        18900: { 'caption': 'AA_0000.jpg' },
                    },
                    annotateEveryFrame: false,
                    callbacks: {
                        "progress": function (stageId, data) {
                            callbacks["progress"](stageId, data);
                        },
                        "done": function (stageId, data) {
                            callbacks["done"](stageId, data);
                        },
                        "annotate": function (stageId, data) {
                            callbacks["annotate"](stageId, data);
                        }
                    }
                }
            ];
    }

    stage(index) {
        let self = this;

        if (typeof self.videoFrame[self.currentConfig] !== "undefined") {
            self.videoFrame[self.currentConfig].stopListen();
        }

        self.currentConfig = index;

        let stage = self.stages[self.currentConfig];
        let videoId = 'stimulus-emotions-' + index;

        $('#stimulus-emotions').attr('id', videoId);
        $('#stimulus-emotions-source').attr('src', stage.file).parent()[0].load();

        self.videoFrame[self.currentConfig] = VideoFrame({
            id: videoId,
            frameRate: stage.frameRate,
            callback: function (frameNumber) {
                let percent = Math.round((frameNumber / self.videoFrame[self.currentConfig].frameRate) * 100 / self.videoFrame[self.currentConfig].video.duration);
                if (stage.callbacks["annotateEveryFrame"]) {
                    stage.callbacks["annotate"](self.currentConfig, { "frame": frameNumber });
                }
                if (typeof stage.callbacks["annotate"] === "function") {
                    if (typeof stage.annotations[frameNumber] === "object") {
                        if (!stage.annotations[frameNumber]["notified"]) {
                            stage.callbacks["annotate"](self.currentConfig, { "caption": stage.annotations[frameNumber].caption });
                            stage.annotations[frameNumber]["notified"] = true;
                        }
                    }
                }
                if (typeof stage.callbacks["progress"] === "function") {
                    stage.callbacks["progress"](self.currentConfig, percent);
                }
                if (percent === 100) {
                    if (typeof stage.callbacks["done"] === "function") {
                        stage.callbacks["done"](self.currentConfig, percent);
                    }
                }
            }
        });

        self.videoFrame[self.currentConfig].stopListen();
        self.videoFrame[self.currentConfig].listen('frame');
    }

    allStages() {
        let result = [];
        let self = this;
        _.each(this.stages, function (stage, index) {
            result.push({ "id": index, "current": index === self.currentConfig, "clickable": true, "name": stage.name });
        });
        return result;
    }

    start() {
        let self = this;
        if (typeof self.videoFrame[self.currentConfig] === "undefined") {
            return;
        }
        self.videoFrame[self.currentConfig].video.play();
    }

    stop() {
        let self = this;
        if (typeof self.videoFrame[self.currentConfig] === "undefined") {
            return;
        }
        self.videoFrame[self.currentConfig].video.pause();
    }

    reset() {
        let self = this;
        if (typeof self.videoFrame[self.currentConfig] === "undefined") {
            return;
        }
        this.stop();
        self.videoFrame[self.currentConfig].video.currentTime = 0;
        _.each(this.stages, function (stage, sindex) {
            _.each(stage.annotations, function (annotate, aindex) {
                annotate.notified = false;
            });
        });
    }

    masterIdle() {
        this.stop();
        this.reset();
    }

}