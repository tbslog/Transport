let routerList = [
  {
    name: "Thiết Lập",
    pathName: "",
    element: undefined,
    exact: false,
    child: [
      {
        name: "Địa Điểm",
        pathName: "/address",
        path: "address",
        exact: true,
        child: [],
      },
      {
        name: "Cung Đường",
        pathName: "/road",
        path: "road",
        exact: true,
        child: [],
      },
      {
        name: "Thông tin tài xế",
        pathName: "/driver",
        path: "driver",
        exact: true,
        child: [],
      },
      {
        name: "Thông tin xe",
        pathName: "/vehicle",
        path: "vehicle",
        exact: true,
        child: [],
      },
      {
        name: "Thông tin romooc",
        pathName: "/romooc",
        path: "romooc",
        exact: true,
        child: [],
      },
    ],
  },
  {
    name: "Đối Tác",
    pathName: "",
    element: undefined,
    exact: false,
    child: [
      {
        name: "Thông Tin Đối Tác",
        pathName: "/custommer",
        path: "custommer",
        exact: true,
        child: [],
      },
      {
        name: "Hợp đồng & Phụ Lục",
        pathName: "/Contract",
        path: "contract",
        exact: true,
        child: [],
      },
      {
        name: "Bảng giá",
        pathName: "/pricetable",
        path: "pricetable",
        exact: true,
        child: [],
      },
      {
        name: "Sản phẩm dịch vụ",
        pathName: "/productservice",
        path: "productservice",
        exact: true,
        child: [],
      },
      {
        name: "Phụ Phí",
        pathName: "/subfee",
        path: "subfee",
        exact: true,
        child: [],
      },
    ],
  },

  {
    name: "Vận Hành",
    pathName: "",
    element: undefined,
    exact: false,
    child: [
      {
        name: "Vận Đơn",
        pathName: "/transport",
        path: "transport",
        exact: true,
        child: [],
      },
      {
        name: "Điều Phối",
        pathName: "/handling",
        path: "handling",
        exact: true,
        child: [],
      },
    ],
  },
  {
    name: "Hóa Đơn & Thống Kê",
    pathName: "",
    element: undefined,
    exact: false,
    child: [
      {
        name: "Hóa Đơn",
        pathName: "/bill",
        path: "bill",
        exact: true,
        child: [],
      },
      {
        name: "Thống Kê",
        pathName: "/report",
        path: "report",
        exact: true,
        child: [],
      },
    ],
  },
  {
    name: "Quản Trị",
    pathName: "",
    element: undefined,
    exact: false,
    child: [
      {
        name: "Người Dùng",
        pathName: "/user",
        path: "user",
        exact: true,
        child: [],
      },
      {
        name: "Phân Quyền",
        pathName: "/role",
        path: "role",
        exact: true,
        child: [],
      },
    ],
  },
];

export function getRouterList() {
  return routerList;
}
