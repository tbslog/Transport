import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import { getData, postFile, getDataCustom, getFile } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import LoadingPage from "../Common/Loading/LoadingPage";
import BillPageCustomer from "./BillPageCustomer";
import BillPageSupplier from "./BillPageSupplier";

const BillPage = () => {
  const [tabIndex, setTabIndex] = useState(0);
  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex);
  };

  return (
    <>
      <Tabs
        selectedIndex={tabIndex}
        onSelect={(index) => HandleOnChangeTabs(index)}
      >
        <TabList>
          <Tab>Hóa Đơn KH</Tab>
          <Tab>Hóa Đơn NCC</Tab>
        </TabList>
        <TabPanel>
          {tabIndex === 0 && <BillPageCustomer></BillPageCustomer>}
        </TabPanel>
        <TabPanel>
          {tabIndex === 1 && <BillPageSupplier></BillPageSupplier>}
        </TabPanel>
      </Tabs>
    </>
  );
};

export default BillPage;
