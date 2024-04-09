const sidebars = {
    docs: [
        {
            type: "category",
            label: "规则",
            link: {
                type: "generated-index",
            },
            collapsed: false,
            items: [
                {
                    type: "category",
                    label: "引入",
                    collapsed: false,
                    link: {
                        type: "generated-index",
                    },
                    items: [
                        "intro/README",
                        "intro/rule",
                        "intro/guide",
                        "intro/programming",
                    ],
                },
                {
                    type: "category",
                    label: "地图",
                    collapsed: false,
                    link: {
                        type: "generated-index",
                    },
                    items: [
                        "map/map",
                        "map/placetype",
                        "map/home",
                        "map/wormhole",
                    ],
                },
                {
                    type: "category",
                    label: "舰船",
                    collapsed: false,
                    link: {
                        type: "generated-index",
                    },
                    items: [
                        "ship/ship",
                        "ship/civilship",
                        "ship/warship",
                        "ship/flagship",
                        "ship/team",
                    ],
                },
                {
                    type: "category",
                    label: "机制",
                    collapsed: false,
                    link: {
                        type: "generated-index",
                    },
                    items: [
                        "mechanics/mechanics",
                        "mechanics/construction",
                        "mechanics/module",
                        "mechanics/view",
                        "mechanics/attack",
                        "mechanics/score",
                    ],
                },
                {
                    type: "category",
                    label: "接口",
                    collapsed: false,
                    link: {
                        type: "generated-index",
                    },
                    items: [
                        "interface/interface",
                        "interface/cpp",
                        "interface/python",
                    ],
                },
            ],
        },
        {
            type: "category",
            label: "常见问题",
            link: {
                type: "generated-index",
            },
            collapsed: false,
            items: [
                "faq/README",
                "faq/cpptips",
            ],
        },
    ],
};

module.exports = sidebars;
