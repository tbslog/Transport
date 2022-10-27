import AddressPage from "./Components/AddressManage/AddressPage";
import CustommerPage from "./Components/CustommerManage/CustommerPage";
import DriverPage from "./Components/DriverManage/DriverPage";
import HomePage from "./Components/Home/HomePage";
import RoadPage from "./Components/RoadManage/RoadPage";
import ContractPage from "./Components/ContractManage/ContractPage";
import PriceTablePage from "./Components/PriceListManage/PriceTablePage";
import TransportPage from "./Components/Transport/TransportPage";
import ProductServicePage from "./Components/ProductService/ProductServicePage";
import VehiclePage from "./Components/VehicleManage/VehiclePage";
import RomoocPage from "./Components/RomoocManage/RomoocPage";

let routerList = [
  {
    name: "Khách hàng",
    pathName: "",
    element: undefined,
    exact: false,
    child: [
      {
        name: "Địa điểm",
        pathName: "/address",
        path: "address",
        element: <AddressPage />,
        exact: true,
        child: [],
      },
      {
        name: "KH & NCC",
        pathName: "/custommer",
        path: "custommer",
        element: <CustommerPage />,
        exact: true,
        child: [],
      },
    ],
  },

  {
    name: "Nhà vận tải",
    pathName: "",
    element: undefined,
    exact: false,
    child: [
      {
        name: "Thông tin chung",
        pathName: "/driver",
        path: "driver",
        element: <DriverPage />,
        exact: true,
        child: [],
      },
      {
        name: "Thông tin xe",
        pathName: "/vehicle",
        path: "vehicle",
        element: <VehiclePage />,
        exact: true,
        child: [],
      },
      {
        name: "Thông tin romooc",
        pathName: "/romooc",
        path: "romooc",
        element: <RomoocPage />,
        exact: true,
        child: [],
      },
      {
        name: "Thông tin tài xế",
        pathName: "/driver",
        path: "driver",
        element: <DriverPage />,
        exact: true,
        child: [],
      },
    ],
  },
  {
    name: "Hợp đồng",
    pathName: "",
    element: undefined,
    exact: false,
    child: [
      {
        name: "Thông tin chung",
        pathName: "/driver",
        path: "driver",
        element: <DriverPage />,
        exact: true,
        child: [],
      },
      {
        name: "Hợp đồng & phụ lục",
        pathName: "/Contract",
        path: "contract",
        element: <ContractPage />,
        exact: true,
        child: [],
      },
      {
        name: "Cung Đường",
        pathName: "/road",
        path: "road",
        element: <RoadPage />,
        exact: true,
        child: [],
      },
      {
        name: "Bảng giá",
        pathName: "/pricetable",
        path: "pricetable",
        element: <PriceTablePage />,
        exact: true,
        child: [],
      },
      {
        name: "Sản phẩm dịch vụ",
        pathName: "/productservice",
        path: "productservice",
        element: <ProductServicePage />,
        exact: true,
        child: [],
      },
    ],
  },
  {
    name: "Vận hành",
    pathName: "",
    element: undefined,
    exact: false,
    child: [
      {
        name: "Vận Đơn (Nhập & Xuất)",
        pathName: "/transport",
        path: "transport",
        element: <TransportPage />,
        exact: true,
        child: [],
      },
      {
        name: "Điều Phối",
        pathName: "/driver",
        path: "driver",
        element: <DriverPage />,
        exact: true,
        child: [],
      },
      {
        name: "Giám sát",
        pathName: "/driver",
        path: "driver",
        element: <DriverPage />,
        exact: true,
        child: [],
      },
      {
        name: "Chứng từ",
        pathName: "/driver",
        path: "driver",
        element: <DriverPage />,
        exact: true,
        child: [],
      },
      {
        name: "Thanh Toán",
        pathName: "/driver",
        path: "driver",
        element: <DriverPage />,
        exact: true,
        child: [],
      },
    ],
  },
];

export function getRouterList() {
  return routerList;
}
